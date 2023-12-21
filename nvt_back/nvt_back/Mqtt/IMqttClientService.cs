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
        public string GetCommandTopicForDevice(int deviceId);
        public string GetDataTopicForDevice(int deviceId);
        public Task PublishActivatedStatus(int deviceId);
        public Task PublishDeactivatedStatus(int deviceId);
        public Task PublishStatusUpdate(int deviceId, string status, int userId);
        public Task PublishRegimeUpdate(int deviceId, string value, int userId);
        public Task SubscribeToHeartbeatTopics();
        public Task SubscribeToCommandTopics();
        public Task SubscribeToDataTopic(int deviceId);
        public Task UnsubscribeFromDataTopic(int deviceId);
        public Task UnsubscribeFromCommandTopic(int deviceId);
    }
}
