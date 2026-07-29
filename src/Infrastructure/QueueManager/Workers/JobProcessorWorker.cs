using Core.ImageProcessor.DTO;
using Core.ImageProcessor.Services;
using Core.QueueManager.Models;
using Core.QueueManager.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.QueueManager.Workers;

public class JobProcessorWorker(
    IServiceProvider sp,
    IJobRepository queue,
    int maxConcurrency = 2) : BackgroundService
{
    private readonly SemaphoreSlim _semaphore = new(maxConcurrency, maxConcurrency);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var jobId = await queue.DequeueAsync(stoppingToken);
            await _semaphore.WaitAsync(stoppingToken);
            _ = Task.Run(() => ProcessJobAsync(jobId, stoppingToken), stoppingToken);
        }
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken ct)
    {
        using var scope = sp.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var job = await db.Jobs.FirstAsync(j => j.Id == jobId, ct);
        try
        {
            var processor = scope.ServiceProvider.GetRequiredService<ImageProcessingService>();

            job.Status = JobStatus.Processing;
            await db.SaveChangesAsync(ct);

            var request = new RequestDto
            {
                JobID = jobId,
                FileName = job.FileName,
                SystemPath = job.OriginalSystemPath,
                CountPoints = job.CountPoints,
                CountSteps = job.CountSteps,
                Padding = job.Padding
            };

            var response = await processor.ProcessImageAsync(request, ct);
            job.ResultImagePath = response.ResultImagePath;
            job.ResultRoutePath = response.ResultRoutePath;
            job.Status = JobStatus.Completed;
            job.Progress = 100;
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.ErrorMessage = ex.Message;
        }
        finally
        {
            job.CompletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            _semaphore.Release();
        }
    }
}
