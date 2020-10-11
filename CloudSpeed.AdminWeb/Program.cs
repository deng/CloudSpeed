using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CloudSpeed.BackgroundServices;
using CloudSpeed.Services;

namespace CloudSpeed.AdminWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(configBuilder =>
            {
                AppSettingsService.AppendConfigurations(configBuilder, "appsettings.json");
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://*:14101").UseStartup<Startup>();
            });
    }
}
