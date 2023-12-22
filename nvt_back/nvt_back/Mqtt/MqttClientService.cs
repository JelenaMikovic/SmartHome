using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.DTOs.Mqtt;
using nvt_back.InfluxDB;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.WebSockets;
using System.Globalization;
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
        private readonly IServiceScopeFactory _scopeFactory;

        public MqttClientService(IOptions<MqttConfiguration> mqttConfiguration, InfluxDBService influxDBService,
            IDeviceOnlineStatusService deviceOnlineStatusService, IDeviceService deviceService,
            IDeviceSimulatorInitializationService deviceSimulatorInitializationService,
            IDeviceRepository deviceRepository, IHubContext<DeviceHub> hubContext, IServiceScopeFactory serviceScopeFactory)
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
            _scopeFactory = serviceScopeFactory;
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
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        var service = serviceProvider.GetRequiredService<IDeviceOnlineStatusService>();
                        await service.UpdateOnlineStatus(heartbeat.DeviceId, true);
                    }
                    await _influxDBService.WriteHeartbeatToInfluxDBForDevice(heartbeat.DeviceId, (int)heartbeat.Status);
                }
            }
            Console.WriteLine(heartbeat.InitializeParameters);
            if (heartbeat.InitializeParameters)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var service = serviceProvider.GetRequiredService<IDeviceSimulatorInitializationService>();
                    var payload = await service.Initialize(heartbeat.DeviceId);
                    await this.Publish(this.GetCommandTopicForDevice(heartbeat.DeviceId), JsonConvert.SerializeObject(payload));
                }
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
                    Console.WriteLine($"{payloadString} data");
                    await handleDataReceived(arg, topic, payloadString);
                    break;
                case "batteries_initialization":
                    handleBatteryInitializationRecieved(arg, topic, payloadString);
                    break;
            }
        }

        private dynamic ParseInfluxDbLine(string line)
        {
            try
            {
                var parts = line.Split(' ');
                Console.WriteLine(parts.Length);
                if (parts.Length == 2)
                {
                    var measurementAndTags = parts[0].Split(',');
                    var measurement = measurementAndTags[0];

                    if (measurement == "command")
                    {
                        var deviceID = int.Parse(measurementAndTags[1].Split('=')[1]);
                        var deviceType = measurementAndTags[2].Split('=')[1];
                        var user = int.Parse(measurementAndTags[3].Split('=')[1]);
                        var type = measurementAndTags[4].Split('=')[1];
                        var success = Boolean.Parse(measurementAndTags[5].Split('=')[1]);

                        if (success)
                            return new
                            {
                                Measurement = measurement,
                                DeviceId = deviceID,
                                User = user,
                                DeviceType = deviceType,
                                Action = type,
                                Value = parts[1].Split('=')[1]
                            };
                    }

                    var deviceId = int.Parse(measurementAndTags[1].Split('=')[1]);

                    if (measurement == "ambiental_sensor")
                    {
                        var values = parts[1].Split(",");
                        var humidity = float.Parse(values[0].Split('=')[1], CultureInfo.InvariantCulture);
                        var temperature = float.Parse(values[1].Split('=')[1], CultureInfo.InvariantCulture);
                        return new
                        {
                            Measurement = measurement,
                            DeviceId = deviceId,
                            Humidity = humidity,
                            Temperature = temperature
                        };
                    }
                    if (measurement == "battery_level")
                    {
                        var battery_level = double.Parse(parts[1].Split('=')[1]);
                        return new
                        {
                            Measurement = measurement,
                            DeviceId = deviceId,
                            Value = battery_level
                        };
                    }
                    if (measurement == "home_battery")
                    {
                    Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                        var consumed_power = double.Parse(parts[1].Split('=')[1]);
                        Console.WriteLine(consumed_power);
                        return new
                        {
                            Measurement = measurement,
                            PropertyId = deviceId,
                            Value = consumed_power
                        };
                    }
                    if (measurement == "ac")
                    {
                        var values = parts[1].Split(",");
                        var mode = values[0].Split('=')[1];
                        var temperature = float.Parse(values[1].Split('=')[1], CultureInfo.InvariantCulture);
                        return new
                        {
                            Measurement = measurement,
                            DeviceId = deviceId,
                            CurrentMode = mode,
                            CurrentTemperature = temperature
                        };
                    }
                    if (measurement == "illuminance")
                    {

                        var illuminance = int.Parse(parts[1].Split('=')[1]);

                        return new MeasurementDTO
                        {
                            Measurement = measurement,
                            DeviceId = deviceId,
                            Value = illuminance
                        };
                    }
                    return null;
                }
                else
                {
                    throw new ArgumentException("Invalid input format");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing InfluxDB line: {ex.Message}");
                throw;
            }

        }

        private async Task handleDataReceived(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            if (payloadString.Contains("plate_update"))
            {
                await handlePlateArrivedUpdate(payloadString);
                return;
            }

            var data = ParseInfluxDbLine(payloadString);

            Device device = null;
            User user = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                device = await repository.GetById(data.DeviceId);
                
            }

            using (var scope = _scopeFactory.CreateScope())
            { 
                var serviceProvider = scope.ServiceProvider;
                var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
                user = await userRepository.GetById(data.User);
                Console.WriteLine(user);
            }

            if (device == null)
            {
                Console.WriteLine("Device with the given id doesn't exist.");
            }

            if(data.Measurement.ToLower() == "command")
            {
                Console.WriteLine("?????????????????????");
                await sendActionUpdate(data, user);
                return;
            }

            if (topic.Split("/")[1] == "property")
            {
                Console.WriteLine(data);
                await sendPowerConsumptionUpdate(data);
            }

            switch (device.DeviceType)
            {
                case DeviceType.LAMP:
                    await sendLampUpdate(data, device);
                    break;
                case DeviceType.AMBIENT_SENSOR:
                    await sendAmbientUpdate(data, device);
                    break;
                case DeviceType.HOME_BATTERY:
                    await sendBatteryUpdate(data, device);
                    break;
                case DeviceType.AC:
                    await sendAcUpdate(data, device);
                    break;
            }


        }

        private PlateUpdateDTO ParseInfluxDbLinePlate(string line)
        {
            var parts = line.Split(' ');

            if (parts.Length != 2)
            {
                Console.WriteLine("PLATE error");
                throw new ArgumentException("Invalid input format");
            }

            var measurement = parts[0].Split(',')[0];
            var deviceId = int.Parse(parts[0].Split('=')[1]);
            var plate = parts[1].Split('=')[1];


            return new PlateUpdateDTO
            {
                Measurement = measurement,
                DeviceId = deviceId,
                Value = plate
            };
        }

        private async Task handlePlateArrivedUpdate(string payloadString)
        {
            var message = ParseInfluxDbLinePlate(payloadString);
            Console.Write("Received update for plate: " + message.Value);
            await _hubContext.Clients.Group($"data/{message.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
        }

        private async Task sendActionUpdate(dynamic data, User user)
        {
            var message = new
            {
                Measurement = data.Measurement,
                DeviceId = data.DeviceId,
                User = user.Name + " " + user.Surname,
                DeviceType = data.DeviceType,
                Action = data.Action,
                Value = data.Value,
                Guid = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
        };
            try
            {
                Console.WriteLine("NEMANJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Console.WriteLine($"data/{data.DeviceId}");
                await _hubContext.Clients.Group($"data/{data.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async Task sendPowerConsumptionUpdate(dynamic data)
        {

            var message = new
            {
                PropertyId = data.PropertyId,
                DeviceType = "HOME_BATTERY",
                Consumed = data.Value,
                Timestamp = DateTime.Now
            };

            Console.WriteLine(message);

            try
            {
                Console.WriteLine($"consumption_data/{data.PropertyId}");
                await _hubContext.Clients.Group($"consumption_data/{data.PropertyId}").SendAsync("ConsumptionUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async Task sendLampUpdate(MeasurementDTO data, Device device)
        {
            Lamp lamp = (Lamp)device;

            if (lamp.BrightnessLevel == (int)data.Value)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                lamp.BrightnessLevel = (int)data.Value;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.SaveChanges(lamp);

            }

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
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async Task sendAmbientUpdate(dynamic data, Device device)
        {
            Console.WriteLine("uso");
            Console.WriteLine(data.Temperature);
            AmbientSensor sensor = (AmbientSensor)device;

            if (sensor.CurrentTemperature == data.Temperature && sensor.CurrentHumidity == data.Humidity)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                sensor.CurrentTemperature = data.Temperature;
                sensor.CurrentHumidity = data.Humidity;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.SaveChanges(sensor);
            }

            var message = new
            {
                DeviceId = data.DeviceId,
                DeviceType = "AMBIENT_SENSOR",
                Temperature = data.Temperature,
                Humidity = data.Humidity,
                Timestamp = DateTime.Now
            };

            try
            {
                Console.WriteLine($"data/{data.DeviceId}");
                await _hubContext.Clients.Group($"data/{data.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async Task sendAcUpdate(dynamic data, Device device)
        {
            AirConditioner sensor = (AirConditioner)device;

            if (sensor.CurrentTemperature == data.CurrentTemperature && sensor.CurrentMode == data.CurrentMode)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                sensor.CurrentTemperature = data.CurrentTemperature;
                sensor.CurrentMode = Enum.Parse<AirConditionerMode>(data.CurrentMode.Replace("\"", ""));
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.SaveChanges(sensor);
            }

            var message = new
            {
                DeviceId = data.DeviceId,
                DeviceType = "AC",
                CurrentTemperature = data.CurrentTemperature,
                CurrentMode = data.CurrentMode.Replace("\"", ""),
            };
            try
            {
                Console.WriteLine($"data/{data.DeviceId}");
                await _hubContext.Clients.Group($"data/{data.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }


        private async Task sendBatteryUpdate(dynamic data, Device device)
        {
            HomeBattery battery = (HomeBattery)device;

            if (battery.CurrentCharge == data.Value)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                battery.CurrentCharge = data.Value;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.SaveChanges(battery);
            }

            var message = new
            {
                DeviceId = data.DeviceId,
                DeviceType = "HOME_BATTERY",
                CurrentCharge = data.Value,
                Timestamp = DateTime.Now
            };

            try
            {
                Console.WriteLine($"data/{data.DeviceId}");
                await _hubContext.Clients.Group($"data/{data.DeviceId}").SendAsync("DataUpdate", JsonConvert.SerializeObject(message));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async void handleBatteryInitializationRecieved(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            int propertyId = 0;
            try
            {
                var payload = JsonConvert.DeserializeObject<PropertyIdPayload>(payloadString)!;
                propertyId = payload.PropertyId;
            } catch (Exception)
            {
                return;
            }
            List<HomeBattery> homeBatteries = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                //battery.CurrentCharge = data.Value;
                var serviceProvider = scope.ServiceProvider;
                var service = serviceProvider.GetRequiredService<IDeviceService>();
                homeBatteries = await service.GetAllBatteriesForPropertyId(propertyId);
            }
            //var homeBatteries = await _deviceService.GetAllBatteriesForPropertyId(propertyId);
            var batteriesInitialization = homeBatteries.Select(battery => new BatteryInitializationDTO
            {
                Id = battery.Id,
                Capacity = battery.Capacity,
                CurrentCharge = battery.CurrentCharge,
            }).ToList();
            Console.Write(batteriesInitialization[0].CurrentCharge);
            var jsonString = JsonConvert.SerializeObject(batteriesInitialization);


            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic/property/" + propertyId + "/command")
                .WithPayload(jsonString)
                .Build();
            if (_mqttClient == null)
                await Connect();
            await _mqttClient.PublishAsync(message);
            Console.WriteLine($"\nPublished message to topic: {"topic/property/"+propertyId+"/command"}");
        }

        private async void handleCommandReceived(MqttApplicationMessageReceivedEventArgs arg, string topic, string payloadString)
        {
            try
            {
                var command = JsonConvert.DeserializeObject<CommandResultDTO>(payloadString);
                // Rest of your code

                if (command.Sender == null || command.Sender == Sender.PLATFORM)
                    return;


                if (command.Result == CommandResult.FAILIURE)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(payloadString);
                    return;
                }

                if (command.Action.ToLower() == "onoff")
                {
                    Console.WriteLine(payloadString);

                    await _deviceRepository.ToggleState(command.DeviceId, command.Value);
                }
                else
                {
                    if (command.Action.ToLower() == "regime")
                    {
                        Console.WriteLine(payloadString);

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var serviceProvider = scope.ServiceProvider;
                            var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                            await _deviceRepository.ToggleState(command.DeviceId, command.Value);

                        }

                        //await _deviceRepository.ToggleState(command.DeviceId, command.Value);
                    }
                    else
                    {
                        if (command.Action == "Regime")
                        {
                            Console.WriteLine(payloadString);
                            await _deviceRepository.ToggleRegime(command.DeviceId, command.Value);
                        }
                    }
                    else
                    {
                        if (command.Action.ToLower() == "open")
                        {
                            Console.WriteLine(payloadString);
                            await _deviceRepository.ToggleCommand(command.DeviceId, command.Action, command.Value);
                        }
                        else
                        {
                            if (command.Action.ToLower() == "private")
                            {
                                Console.WriteLine(payloadString);
                                await _deviceRepository.ToggleCommand(command.DeviceId, command.Action, command.Value);
                                return;
                            }

                            if (command.Action.ToLower() == "addplate")
                            {
                                await _deviceRepository.UpdateGateAllowedPlates(command.DeviceId, command.Value, true);
                                return;
                            }

                            if (command.Action.ToLower() == "removeplate")
                            {
                                await _deviceRepository.UpdateGateAllowedPlates(command.DeviceId, command.Value, false);
                                return;
                            }
                        }
                    }

                }

            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                Console.WriteLine(payloadString);
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

        public async Task SubscribeToHomeBatteryTopics()
        {
            Console.WriteLine("Subscribing to home batteries...");
            List<int> propertyIds = await _deviceService.GetPropertyIdsWithHomeBatteries();
            var subscriptionTasks = propertyIds
                .Select(id => this.Subscribe(this.GetHomeBatteryInitializationTopicForProperty(id)))
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

        public async Task SubscribeToPropertyDataTopic(int deviceId)
        {
            Console.WriteLine("Subscribing to property's data topic...");
            await this.Subscribe("topic/property/" + deviceId + "/data");
        }


        public async Task UnsubscribeFromDataTopic(int deviceId)
        {
            string topic = this.GetDataTopicForDevice(deviceId);
            Console.WriteLine($"\nUnsubscribed from topic: {topic}");
            await this.Unsubscribe(topic);
        }

        public async Task UnsubscribeFromCommandTopic(int deviceId)
        {
            string topic = this.GetCommandTopicForDevice(deviceId);
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

        public string GetHomeBatteryInitializationTopicForProperty(int propertyId)
        {
            return "topic/property/" + propertyId + "/batteries_initialization";
        }

        public string GetHomeBatteryConsumptionTopicForProperty(int propertyId)
        {
            return "topic/property/" + propertyId + "/command";
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
            if(userId == 0)
            {
                var payload = new
                {
                    Type = "OnOff",
                    Sender = Sender.PLATFORM,
                    Action = status,
                    Actor = "Schedule"
                };
                var payloadJSON = JsonConvert.SerializeObject(payload);
                await this.Publish(topic, payloadJSON);
            } 
            else 
            {
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

        public async Task PublishModeUpdate(int deviceId, string value, int userId)
        {
            if(userId == 0)
            {
                string topic = GetCommandTopicForDevice(deviceId);
                var payload = new
                {
                    Type = "ChangeMode",
                    Sender = Sender.PLATFORM,
                    Value = value,
                    Actor = "Schedule"
                };
                var payloadJSON = JsonConvert.SerializeObject(payload);
                await this.Publish(topic, payloadJSON);
            }
            else
            {
                string topic = GetCommandTopicForDevice(deviceId);
                var payload = new
                {
                    Type = "ChangeMode",
                    Sender = Sender.PLATFORM,
                    Value = value,
                    Actor = userId
                };
                var payloadJSON = JsonConvert.SerializeObject(payload);
                await this.Publish(topic, payloadJSON);
            }
            
        }

        public async Task PublishTemperatureUpdate(int deviceId, string value, int userId)
        {
            if (userId == 0)
            {
                string topic = GetCommandTopicForDevice(deviceId);
                var payload = new
                {
                    Type = "ChangeTemperature",
                    Sender = Sender.PLATFORM,
                    Value = value,
                    Actor = "Schedule"
                };
                var payloadJSON = JsonConvert.SerializeObject(payload);
                await this.Publish(topic, payloadJSON);
            }
            else
            {
                string topic = GetCommandTopicForDevice(deviceId);
                var payload = new
                {
                    Type = "ChangeTemperature",
                    Sender = Sender.PLATFORM,
                    Value = value,
                    Actor = userId
                };
                var payloadJSON = JsonConvert.SerializeObject(payload);
                await this.Publish(topic, payloadJSON);
            }
        }

        public async Task PublishCommandUpdate(int deviceId, string type, string value, int userId)
        {
            string topic = GetCommandTopicForDevice(deviceId);
            var payload = new
            {
                Type = type,
                Sender = Sender.PLATFORM,
                Value = value,
                Actor = userId
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);
            await this.Publish(topic, payloadJSON);

        }
    }

    class PropertyIdPayload
    {
        public int PropertyId { get; set; }
    }
}
