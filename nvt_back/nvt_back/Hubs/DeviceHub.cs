using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using nvt_back.Mqtt;

namespace nvt_back.WebSockets
{
    [Authorize]
    public class DeviceHub : Hub
    {
        private readonly IMqttClientService _mqttClientService;

        public DeviceHub(IMqttClientService mqttService)
        {
            _mqttClientService = mqttService;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("connected! ----------------");
            return base.OnConnectedAsync();
        }

        public async Task SubscribeToDataTopic(int deviceId)
        {
            Console.WriteLine($"data/{deviceId}");
            await _mqttClientService.SubscribeToDataTopic(deviceId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"data/{deviceId}");
        }

        public async Task SubscribeToPropertyDataTopic(int propertyId)
        {
            Console.WriteLine($"OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            await _mqttClientService.SubscribeToPropertyDataTopic(propertyId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"consumption_data/{propertyId}");
        }

        public async Task SubscribeToCommandTopic(int deviceId)
        {
            Console.WriteLine($"command/{deviceId}");
            await _mqttClientService.SubscribeToDataTopic(deviceId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"command/{deviceId}");
        }
        public async Task UnsubscribeFromDataTopic(int deviceId)
        {
            Console.WriteLine($"Unsubscribe from data/{deviceId}");
            await _mqttClientService.UnsubscribeFromDataTopic(deviceId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"data/{deviceId}");
        }

        public async Task UnsubscribeFromCommandTopic(int deviceId)
        {
            Console.WriteLine($"Unsubscribe from command/{deviceId}");
            await _mqttClientService.UnsubscribeFromCommandTopic(deviceId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"command/{deviceId}");
        }

    }
}
