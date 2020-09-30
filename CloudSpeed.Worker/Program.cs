using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudSpeed.BackgroundServices;
using CloudSpeed.Services;
using CloudSpeed.Web;

namespace CloudSpeed.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = AppSettingsService.BuildConfiguration("appsettings.json");
            CreateHostBuilder(config, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfigurationRoot config, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAppServices(config);
                    services.AddHostedService<CloudSpeed.BackgroundServices.Worker>();
                    services.BuildGlobalServices();
                });
    }
}
