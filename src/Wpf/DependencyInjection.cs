using Core;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Services;
using Wpf.Windows.Main;

namespace Wpf;

public static class DependencyInjection
{
    public static IServiceProvider AddServices(this IHostApplicationBuilder builder)
    {
        builder.AddApplicationServices();
        builder.AddInfrastructureServices();

        builder.Services.AddSingleton<IDialogService, DialogService>();

        //// Explorer
        //builder.Services.AddTransient<ExplorerVM>();
        //// Editor
        //builder.Services.AddTransient<EditorVM>();
        //// Recent projects
        //builder.Services.AddTransient<RecentProjectsVM>();

        // Windows
        builder.Services.AddTransient<MainWindow>();

        // ViewModels
        builder.Services.AddTransient<MainWindowVM>();

        return builder.Services.BuildServiceProvider();
    }
}
