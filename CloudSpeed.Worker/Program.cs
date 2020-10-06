using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CloudSpeed.Services;
using CloudSpeed.Web;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CloudSpeed.Sdk;
using CloudSpeed.Powergate;

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
                    services.AddSingleton<CloudSpeed.BackgroundServices.LotusWorker>();
                    services.AddSingleton<CloudSpeed.BackgroundServices.PowergateWorker>();
                    services.AddHostedService<BackgroundService>(sp =>
                    {
                        var lcs = sp.GetService<LotusClientSetting>();
                        var pgs = sp.GetService<PowergateSetting>();
                        
                        if (lcs.Enabled && pgs.Enabled)
                            throw new System.Exception("LotusClientSetting, PowergateSetting only one enabled");

                        if (lcs.Enabled)
                            return sp.GetService<CloudSpeed.BackgroundServices.LotusWorker>();

                        return sp.GetService<CloudSpeed.BackgroundServices.PowergateWorker>();
                    });
                    services.BuildGlobalServices();
                });
   }
}
