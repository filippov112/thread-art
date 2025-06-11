using Microsoft.AspNetCore.SignalR;

public class ProgressHub : Hub
{
    public async Task SendProgress(int progress)
    {
        await Clients.All.SendAsync("ReceiveProgress", progress);
    }
}