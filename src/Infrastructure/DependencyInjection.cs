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

        // Services
        builder.Services.AddHostedService<FileCleanupService>();
        builder.Services.AddTransient<IPainter, Painter>();
        builder.Services.AddTransient<IProgressLogger, ProgressLoggerAdapter>();
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        // Repositories
        builder.Services.AddScoped<IProcessedResultRepository, ProcessedResultRepository>();
    }
}
