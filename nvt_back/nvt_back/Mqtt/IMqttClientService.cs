using MQTTnet.Protocol;

namespace nvt_back.Mqtt
{
    public interface IMqttClientService
    {
        public Task Connect();
        public Task Subscribe(string topic);
        public Task Unsubscribe(string topic);
        public Task Publish(string topic, string payload, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, bool retain = false);
        public string GetHeartbeatTopicForDevice(int deviceId);
        public Task PublishActivatedStatus(int deviceId);
        public Task PublishDeactivatedStatus(int deviceId);
        public Task SubscribeToHeartbeatTopics();
    }
}
