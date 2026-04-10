using Microsoft.AspNetCore.SignalR;

namespace Infrastructure
{
    public class ProgressHub : Hub
    {
        public async Task SendProgress(int progress)
        {
            await Clients.All.SendAsync("ReceiveProgress", progress);
        }
    }
}