using System.Linq;
using CloudSpeed.Managers;
using CloudSpeed.Powergate;
using CloudSpeed.Repositories;
using CloudSpeed.Sdk;
using CloudSpeed.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CloudSpeed.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = LoggerFactory
                .Create(builder => builder.AddFilter(FilterEntityFrameworkCore).AddLog4Net(configuration.CombineConfigPath("log4net.config")));
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.BindConfiguration(configuration);
            services.AddSingleton<LotusClient>();
            services.UseRepositories();
            services.AddSingleton<IPanPasswordHasher, PanPasswordHasher>();
            services.AddSingleton<CloudSpeedManager>();
            services.UsePowergateRrpcServices(configuration);
            return services;
        }

        private static bool FilterEntityFrameworkCore(string category, Microsoft.Extensions.Logging.LogLevel level)
        {
            var filtedCategories = new[]
            {
                DbLoggerCategory.Database.Command.Name,
                "Microsoft.EntityFrameworkCore.Infrastructure"
            };
            return !(filtedCategories.Contains(category) && level == Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}
