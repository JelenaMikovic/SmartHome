using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using nvt_back.Mqtt;

namespace nvt_back.WebSockets
{
    [Authorize]
    public class DeviceHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("connected! ----------------");
            return base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string message)
        {

        }
    }
}
