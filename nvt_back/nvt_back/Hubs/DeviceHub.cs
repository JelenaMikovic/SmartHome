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


    }
}
