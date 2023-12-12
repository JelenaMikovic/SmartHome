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
                await mqttClientService.Connect();
                await mqttClientService.SubscribeToHeartbeatTopics();
            }
            else
            {
                Console.WriteLine("MqttClientService is null!");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
