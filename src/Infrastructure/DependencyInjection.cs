using Core.ImageProcessor.Services;
using Core.QueueManager.Repositories;
using Core.Repositories;
using Infrastructure.Data;
using Infrastructure.ImageProcessor.Services;
using Infrastructure.ImageProcessor.Settings;
using Infrastructure.QueueManager.Repositories;
using Infrastructure.QueueManager.Settings;
using Infrastructure.QueueManager.Workers;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Storage
        builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));

        // DB
        builder.Services.AddDbContext<ApplicationDbContext>();

        // Устанавливаем число паралельных задач
        var processingConfig = builder.Configuration.GetSection(ProcessingOptions.SectionName).Get<ProcessingOptions>();
        int maxConcurrency;
        if (processingConfig != null && processingConfig.MaxConcurrency > 0)
            maxConcurrency = processingConfig.MaxConcurrency;
        else
            maxConcurrency = Math.Max(1, Environment.ProcessorCount - 1); // Авто-расчет: Кол-во ядер минус 1 (оставляем запас для IO/системы)

        // Services
        builder.Services.AddSingleton<IJobRepository, JobRepository>();
        builder.Services.AddHostedService(sp =>
        {
            var queue = sp.GetRequiredService<IJobRepository>();
            return new JobProcessorWorker(sp, queue, maxConcurrency);
        });
        builder.Services.AddTransient<IFileSystemService, FileSystemService>();

        // Repositories
        builder.Services.AddScoped<IProcessingJobRepository, ProcessingJobRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    }
}
