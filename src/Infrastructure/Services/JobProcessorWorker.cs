using Application.DTO;
using Application.Interfaces;
using Application.UseCases;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class JobProcessorWorker(IServiceProvider sp, IJobQueue queue, int maxConcurrency = 2) : BackgroundService
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
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var job = await db.Jobs.FirstAsync(j => j.Id == jobId, ct);
        try
        {
            var processor = scope.ServiceProvider.GetRequiredService<ImageProcessor>();

            job.Status = JobStatus.Processing;
            await db.SaveChangesAsync(ct);

            var request = new ProcessingRequest
            {
                FileName = job.FileName,
                SystemPath = job.OriginalSystemPath,
                WebPath = job.OriginalWebPath,
                CountPoints = job.CountPoints,
                CountSteps = job.CountSteps,
                ContrastLine = job.ContrastLine
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
