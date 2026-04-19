using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class ProgressLoggerAdapter : IProgressLogger
{
    private readonly IHubContext<ProgressHub> _hubContext;

    public ProgressLoggerAdapter(IHubContext<ProgressHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendProgressAsync(ProgressStage stage)
    {
        // Отправляем всем подключенным клиентам
        await _hubContext.Clients.All.SendAsync("ReceiveProgress", (int)stage);
    }
}
