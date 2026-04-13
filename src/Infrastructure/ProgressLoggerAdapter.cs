using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure;

public class ProgressLoggerAdapter : IProgressLogger
{
    private readonly IHubContext<ProgressHub> _hubContext;

    public ProgressLoggerAdapter(IHubContext<ProgressHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendProgress(int progress)
    {
        // Отправляем всем подключенным клиентам
        await _hubContext.Clients.All.SendAsync("ReceiveProgress", progress);
    }
}
