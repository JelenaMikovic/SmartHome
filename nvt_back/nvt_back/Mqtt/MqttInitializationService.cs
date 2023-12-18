namespace nvt_back.Mqtt
{
    public class MqttInitializationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MqttInitializationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var mqttClientService = scope.ServiceProvider.GetService<IMqttClientService>();

            if (mqttClientService != null)
            {
                try
                {
                    await mqttClientService.Connect();
                    await mqttClientService.SubscribeToHeartbeatTopics();
                    await mqttClientService.SubscribeToCommandTopics();
                } catch (Exception ex)
                {
                    Console.WriteLine("Unable to connect to MQTT");
                }
            }
            else
            {
                Console.WriteLine("MqttClientService is null!");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
