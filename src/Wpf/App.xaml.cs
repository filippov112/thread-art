using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Windows.Projects;

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
            var projects = _provider.Services.GetRequiredService<ProjectsWindow>();
            projects.Show();
        }
    }

}
