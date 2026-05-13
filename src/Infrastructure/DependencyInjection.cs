using Application.Interfaces;
using Application.Repositories;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<StorageOptions>(
            builder.Configuration.GetSection(StorageOptions.SectionName)
            );
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Infrastructure")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Устанавливаем число паралельных задач
        var processingConfig = builder.Configuration.GetSection(ProcessingOptions.SectionName).Get<ProcessingOptions>();
        int maxConcurrency;
        if (processingConfig != null && processingConfig.MaxConcurrency > 0)
            maxConcurrency = processingConfig.MaxConcurrency;
        else
            maxConcurrency = Math.Max(1, Environment.ProcessorCount - 1); // Авто-расчет: Кол-во ядер минус 1 (оставляем запас для IO/системы)

        // Services
        builder.Services.AddSingleton<IJobQueue, MemoryJobQueue>();
        builder.Services.AddHostedService(sp =>
        {
            var queue = sp.GetRequiredService<IJobQueue>();
            return new JobProcessorWorker(sp, queue, maxConcurrency);
        });
        builder.Services.AddHostedService<FileCleanupService>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<IFileSystemService, FileSystemService>();

        // Repositories
        builder.Services.AddScoped<IImageModelRepository, ImageModelRepository>();
        builder.Services.AddScoped<IProcessingJobRepository, ProcessingJobRepository>();
    }
}
