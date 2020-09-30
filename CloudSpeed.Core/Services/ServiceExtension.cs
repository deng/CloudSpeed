using CloudSpeed.Managers;
using CloudSpeed.Repositories;
using CloudSpeed.Sdk;
using CloudSpeed.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudSpeed.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindConfiguration(configuration);
            services.AddSingleton<LotusClient>();
            services.UseRepositories();
            services.AddSingleton<IPanPasswordHasher, PanPasswordHasher>();
            services.AddSingleton<CloudSpeedManager>();
            return services;
        }
    }
}
