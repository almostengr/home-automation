using Almostengr.InternetMonitor.Model;
using Almostengr.InternetMonitor.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Almostengr.InternetMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()   // specific for linux systems
                .UseContentRoot(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location))
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AppSettings appSettings = configuration.GetSection(nameof(appSettings)).Get<AppSettings>();
                    services.AddSingleton(appSettings);

                    services.AddHostedService<DdWrtRouterWorker>();
                    services.AddHostedService<ArrisModemWorker>();
                });
    }
}
