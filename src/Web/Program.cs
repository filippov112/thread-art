using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Infrastructure")));

// Services
builder.Services.AddSingleton<IConfigurationBuilder, ConfigurationBuilder>();
builder.Services.AddTransient<ImageProcessor>();
builder.Services.AddTransient<IPainter, Painter>();
builder.Services.AddTransient<IRouteRenderer, RouteRenderer>();
builder.Services.AddTransient<IProgressLogger, ProgressLoggerAdapter>();
builder.Services.AddHostedService<FileCleanupService>();

// Repositories
builder.Services.AddScoped<IProcessedResultRepository, ProcessedResultRepository>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("Миграции успешно применены.");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при применении миграций.");
        throw;
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHub<ProgressHub>("/progressHub");
app.MapControllers();
app.Run();
