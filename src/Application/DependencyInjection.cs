using System.Reflection;
using Application.Interfaces;
using Application.Services;
using Application.UseCases;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<ImageProcessor>();
        builder.Services.AddTransient<IRouteRenderer, RouteRenderer>();

        builder.Services.AddAutoMapper(cfg =>
            cfg.AddMaps(Assembly.GetExecutingAssembly()));
    }
}
