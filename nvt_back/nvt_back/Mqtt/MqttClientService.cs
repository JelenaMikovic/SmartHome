﻿using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using nvt_back.InfluxDB;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
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

        public MqttClientService(IOptions<MqttConfiguration> mqttConfiguration, InfluxDBService influxDBService,
            IDeviceOnlineStatusService deviceOnlineStatusService, IDeviceService deviceService)
        {
            var config = mqttConfiguration.Value;
            _username = config.Username;
            _password = config.Password;
            _host = config.Host;
            _port = config.Port;
            _influxDBService = influxDBService;
            _deviceOnlineStatusService = deviceOnlineStatusService;
            _deviceService = deviceService;
        }

        public async Task Connect()
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            _mqttClient = mqttClient;

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
            Console.WriteLine("\nDisconnected from MQTT, with reason: " + arg.ConnectResult.ReasonString);
            return Task.CompletedTask;
        }

        private Task _mqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Console.WriteLine("Connecting to MQTT...");
            return Task.CompletedTask;
        }

        private async Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string topic = arg.ApplicationMessage.Topic;
            if (topic.Split("/").Last() == "heartbeat")
            {
                string payloadString = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                Console.WriteLine($"\nGot message {payloadString} from topic {topic}");

                var heartbeat = JsonConvert.DeserializeObject<Heartbeat>(payloadString)!;
                bool hasStatusChanged = await _deviceOnlineStatusService.HasDeviceOnlineStatusChanged(heartbeat.DeviceId, heartbeat.Status == Status.ON);

                if (heartbeat.Status == Status.ON)
                {
                    await _deviceOnlineStatusService.UpdateLatestHeartbeat(heartbeat, DateTime.UtcNow);

                    if (hasStatusChanged) {
                        await _deviceOnlineStatusService.UpdateOnlineStatus(heartbeat.DeviceId, true);
                        await _influxDBService.WriteHeartbeatToInfluxDBForDevice(heartbeat.DeviceId, (int)heartbeat.Status);
                    }
                }
            }
        }

        public async Task Subscribe(string topic)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Console.WriteLine($"Subscribed to topic: {topic}");
        }

        public async Task SubscribeToHeartbeatTopics()
        {
            Console.WriteLine("Subscribing to online devices...");
            var devices = await _deviceService.GetAll();
            var subscriptionTasks = devices
                .Select(device => this.Subscribe(this.GetHeartbeatTopicForDevice(device.Id)))
                .ToList();
            await Task.WhenAll(subscriptionTasks);
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
                .WithQualityOfServiceLevel(qos)
                .WithRetainFlag(retain)
                .Build();

            await _mqttClient.PublishAsync(message);
            Console.WriteLine($"\nPublished message to topic: {topic}");
        }

        public string GetHeartbeatTopicForDevice(int deviceId)
        {
            return "topic/device/" + deviceId + "/heartbeat";
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

        public async Task PublishDeactivatedStatus(int deviceId)
        {
            string topic = GetHeartbeatTopicForDevice(deviceId);
            var payload = new Heartbeat
            {
                PlatformResponse = "You are offline!",
                Sender = Sender.PLATFORM,
                Status = Status.OFF
            };
            var payloadJSON = JsonConvert.SerializeObject(payload);

            await this.Publish(topic, payloadJSON);
        }
    }
}