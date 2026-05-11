using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class ProgressLoggerAdapter(IHubContext<ProgressHub> hubContext) : IProgressLogger
{
    public async Task SendProgressAsync(ProgressStage stage)
    {
        // Отправляем всем подключенным клиентам
        await hubContext.Clients.All.SendAsync("ReceiveProgress", (int)stage);
    }
}
