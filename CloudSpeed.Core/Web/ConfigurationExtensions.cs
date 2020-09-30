using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using CloudSpeed.Settings;
using CloudSpeed.Sdk;

namespace CloudSpeed.Web
{
    public static class ConfigurationExtensions
    {
        public static void BindConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FilePanDbSetting>(configuration.GetSection(nameof(FilePanDbSetting)),
                (serviceProvider, setting) =>
                {
                    if (setting == null || string.IsNullOrEmpty(setting.Connection))
                        throw new ApplicationException("please setup /FilePanDbSetting at appsettings.json");

                    return setting;
                });

            services.Configure<LotusClientSetting>(configuration.GetSection(nameof(LotusClientSetting)),
                (serviceProvider, setting) =>
                {
                    if (setting == null || string.IsNullOrEmpty(setting.LotusApi) || string.IsNullOrEmpty(setting.LotusToken))
                        throw new ApplicationException("please setup /LotusClientSetting at appsettings.json");

                    return setting;
                });

            services.Configure<UploadSetting>(configuration.GetSection(nameof(UploadSetting)),
                (serviceProvider, setting) =>
                {
                    if (setting == null || setting.Storages == null || setting.Storages.Length == 0)
                        throw new ApplicationException("please setup /UploadSetting at appsettings.json");

                    return setting;
                });
        }

        public static IServiceCollection Configure<TSetting>(this IServiceCollection services,
            IConfigurationSection section, Func<IServiceProvider, TSetting, TSetting> assertSetting) where TSetting : class
        {
            return services.AddSingleton(provider =>
            {
                var setting = section.Get<TSetting>();
                return assertSetting(provider, setting);
            });
        }
    }
}
