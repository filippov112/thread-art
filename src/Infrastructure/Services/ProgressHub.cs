using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class ProgressHub : Hub
    {
        public async Task SendProgressAsync(int progress)
        {
            await Clients.All.SendAsync("ReceiveProgress", progress);
        }
    }
}
