using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.DTOs.Mqtt;
using nvt_back.InfluxDB;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.WebSockets;
using System.Text;

namespace nvt_back.Mqtt
{
    public class MqttConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class MqttClientService : IMqttClientService
    {
        private  IMqttClient _mqttClient;
        private  string _username;
        private  string _password;
        private  string _host;
        private  int _port;

        private readonly InfluxDBService _influxDBService;
        private readonly IDeviceOnlineStatusService _deviceOnlineStatusService;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly IDeviceSimulatorInitializationService _deviceSimulatorInitializationService;

        public MqttClientService(IOptions<MqttConfiguration> mqttConfiguration, InfluxDBService influxDBService,
            IDeviceOnlineStatusService deviceOnlineStatusService, IDeviceService deviceService,
            IDeviceSimulatorInitializationService deviceSimulatorInitializationService,
            IDeviceRepository deviceRepository, IHubContext<DeviceHub> hubContext)
        {
            var config = mqttConfiguration.Value;
            _username = config.Username;
            _password = config.Password;
            _host = config.Host;
            _port = config.Port;
            _influxDBService = influxDBService;
            _deviceOnlineStatusService = deviceOnlineStatusService;
            _deviceService = deviceService;
            _deviceSimulatorInitializationService = deviceSimulatorInitializationService;
            _deviceRepository = deviceRepository;
            _hubContext = hubContext;
        }

        public async Task Connect()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_host, _port)
                .WithCredentials(_username, _password)
                .Build();


            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync;

            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;

            _mqttClient.ConnectingAsync += _mqttClient_ConnectingAsync;

            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync;

            await _mqttClient.ConnectAsync(options);
        }

        private Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("Connection to MQTT complete with code: " + arg.ConnectResult.ResultCode);
            return Task.CompletedTask;
        }

        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            //Console.WriteLine("\nDisconnected from MQTT, with reason: " + arg.ConnectResult.ReasonString);
            return Task.CompletedTask;
        }

        private Task _mqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Console.WriteLine("Connecting to MQTT...");
            return Task.CompletedTask;
        }

        private async void handleHeartbeatReceived(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            var heartbeat = JsonConvert.DeserializeObject<Heartbeat>(payloadString)!;
            bool hasStatusChanged = await _deviceOnlineStatusService.HasDeviceOnlineStatusChanged(heartbeat.DeviceId, heartbeat.Status == Status.ON);

            if (heartbeat.Status == Status.ON)
            {
                await _deviceOnlineStatusService.UpdateLatestHeartbeat(heartbeat, DateTime.UtcNow);

                if (hasStatusChanged)
                {
                    await _deviceOnlineStatusService.UpdateOnlineStatus(heartbeat.DeviceId, true);
                    await _influxDBService.WriteHeartbeatToInfluxDBForDevice(heartbeat.DeviceId, (int)heartbeat.Status);
                }
            }
            Console.WriteLine(heartbeat.InitializeParameters);
            if (heartbeat.InitializeParameters)
            {
                var payload = await _deviceSimulatorInitializationService.Initialize(heartbeat.DeviceId);
                await this.Publish(this.GetCommandTopicForDevice(heartbeat.DeviceId), JsonConvert.SerializeObject(payload));
            }
        }

        private async Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string topic = arg.ApplicationMessage.Topic;
            string topicType = topic.Split("/").Last();

            string payloadString = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"\nGot message {payloadString} from topic {topic}");

            switch (topicType)
            {
                case "heartbeat":
                    handleHeartbeatReceived(arg, topic, payloadString);
                    break;
                case "command":
                    handleCommandReceived(arg, topic, payloadString);
                    break;
                case "data":
                    await handleDataReceived(arg, topic, payloadString);
                    break;
            }
        }

        private MeasurementDTO ParseInfluxDbLine(string line)
        {
            var parts = line.Split(' ');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid input format");
            }

            var measurement = parts[0].Split(',')[0];
            var deviceId = int.Parse(parts[0].Split('=')[1]);
            var illuminance = int.Parse(parts[1].Split('=')[1]);

            return new MeasurementDTO
            {
                Measurement = measurement,
                DeviceId = deviceId,
                Value = illuminance
            };
        }

        private async Task handleDataReceived(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            var data = ParseInfluxDbLine(payloadString);
            Console.WriteLine(data.DeviceId);
            Console.WriteLine("evo" + data.Value);

            //Device device = await _deviceRepository.GetById(data.DeviceId);

            //if (device == null)
            //{
            //    Console.WriteLine("Device with the given id doesn't exist.");
            //}

            //Console.WriteLine(device.DeviceType);
            //Console.WriteLine(device.Id);

            //switch (device.DeviceType)
            //{
            //    case DeviceType.LAMP:
            //        await sendLampUpdate(data, device);
            //        break;

            //}

            await sendLampUpdate(data);

        }

        private async Task sendLampUpdate(MeasurementDTO data)
        {
            var message = new
            {
                DeviceId = data.DeviceId,
                DeviceType = "LAMP",
                Value = data.Value
            };

            try
            {
                Console.WriteLine($"data/{data.DeviceId}");
                await _hubContext.Clients.Group($"data/{data.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                // Log or print the exception details
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
            //await _hubContext.Clients.All.SendAsync("illumUpdate", JsonConvert.SerializeObject(message));
        }

        //private async Task sendLampUpdate(MeasurementDTO data, Device device)
        //{
        //    Lamp lamp = (Lamp)device;

              //provera da li se razlikuje od prethodne vrednosti, samo tada salji

        //    lamp.BrightnessLevel = (int)data.Value;
        //    Console.WriteLine(lamp.BrightnessLevel);
        //    await _deviceRepository.SaveChanges();

        //}

        private async void handleCommandReceived(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            var command = JsonConvert.DeserializeObject<CommandResultDTO>(payloadString)!;

            if (command.Sender == Sender.PLATFORM)
                return;

            if (command.Result == CommandResult.FAILIURE)
            {
                Console.WriteLine("Error");
                Console.WriteLine(payloadString);

                //TODO: posalji na front preko soketa
            }

            if (command.Action == "OnOff")
            {
                //handle device turn off / on
                Console.WriteLine("Evo porukice SVE OK *****************");
                Console.WriteLine(payloadString);

                await _deviceRepository.ToggleState(command.DeviceId, command.Value);
            } 
            else
            {
                if (command.Action == "Regime")
                {
                    Console.WriteLine("Evo porukice SVE OK *****************");
                    Console.WriteLine(payloadString);

                    await _deviceRepository.ToggleRegime(command.DeviceId, command.Value);
                }
            }
        }

        public async Task Subscribe(string topic)
        {
            if (_mqttClient == null)
                await Connect();
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Console.WriteLine($"Subscribed to topic: {topic}");
        }

        public async Task SubscribeToHeartbeatTopics()
        {
            Console.WriteLine("Subscribing to devices' heartbeat topic...");
            var devices = await _deviceService.GetAll();
            var subscriptionTasks = devices
                .Select(device => this.Subscribe(this.GetHeartbeatTopicForDevice(device.Id)))
                .ToList();
            await Task.WhenAll(subscriptionTasks);
        }

        public async Task SubscribeToCommandTopics()
        {
            Console.WriteLine("Subscribing to devices' command topic...");
            var devices = await _deviceService.GetAll();
            var subscriptionTasks = devices
                .Select(device => this.Subscribe(this.GetCommandTopicForDevice(device.Id)))
                .ToList();
            await Task.WhenAll(subscriptionTasks);
        }

        public async Task SubscribeToDataTopic(int deviceId)
        {
            Console.WriteLine("Subscribing to device's data topic...");
            await this.Subscribe(this.GetDataTopicForDevice(deviceId));
        }

        public async Task UnsubscribeFromDataTopic(int deviceId)
        {
            string topic = this.GetDataTopicForDevice(deviceId);
            Console.WriteLine($"\nUnsubscribed from topic: {topic}");
            await this.Unsubscribe(topic);
        }

        public async Task Unsubscribe(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);
            Console.WriteLine($"\nUnsubscribed from topic: {topic}");
        }

        public async Task Publish(string topic, string payload, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, bool retain = false)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag()
                .Build();
            if (_mqttClient == null)
                await Connect();
            await _mqttClient.PublishAsync(message);
            Console.WriteLine($"\nPublished message to topic: {topic}");
        }

        public string GetHeartbeatTopicForDevice(int deviceId)
        {
            return "topic/device/" + deviceId + "/heartbeat";
        }

        public string GetCommandTopicForDevice(int deviceId)
        {
            return "topic/device/" + deviceId + "/command";
        }

        public string GetDataTopicForDevice(int deviceId)
        {
            return "topic/device/" + deviceId + "/data";
        }

        public async Task PublishActivatedStatus(int deviceId)
        {
            string topic = GetHeartbeatTopicForDevice(deviceId);
            var payload = new Heartbeat
            {
                PlatformResponse = "You are online!",
                Sender = Sender.PLATFORM,
                Status = Status.ON
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);
            
            await this.Publish(topic, payloadJSON);
        }

        // TODO: kako ovo?
        public async Task PublishDeactivatedStatus(int deviceId)
        {
            string topic = GetCommandTopicForDevice(deviceId);
            var payload = new 
            {
                Type = "OnlineOffline",
                Sender = Sender.PLATFORM,
                Action = "offline"
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);

            await this.Publish(topic, payloadJSON);
        }

        public async Task PublishStatusUpdate(int deviceId, string status, int userId)
        {
            string topic = GetCommandTopicForDevice(deviceId);
            var payload = new
            {
                Type = "OnOff",
                Sender = Sender.PLATFORM,
                Action = status,
                Actor = userId
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);
            await this.Publish(topic, payloadJSON);

        }

        public async Task PublishRegimeUpdate(int deviceId, string value, int userId)
        {
            string topic = GetCommandTopicForDevice(deviceId);
            var payload = new
            {
                Type = "Regime",
                Sender = Sender.PLATFORM,
                Value = value,
                Actor = userId
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);
            await this.Publish(topic, payloadJSON);

        }
    }
}
