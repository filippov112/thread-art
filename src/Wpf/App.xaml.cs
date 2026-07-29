using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Windows.Main;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _provider;
        public App()
        {
            var builder = new HostApplicationBuilder();
            builder.AddServices();
            _provider = builder.Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = _provider.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
