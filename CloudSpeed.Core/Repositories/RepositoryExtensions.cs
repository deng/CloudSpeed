using System;
using CloudSpeed.Entities;
using CloudSpeed.Identity;
using CloudSpeed.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CloudSpeed.Repositories
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection UseRepositories(this IServiceCollection services)
        {
            services.UseDbContext();
            services.AddScoped<ICloudSpeedRepository, CloudSpeedRepository>();
            return services;
        }

        public static IServiceCollection UseDbContext(this IServiceCollection services)
        {
            services.AddDbContext<CloudSpeedDbContext>((serviceProvider, options) =>
            {
                var setting = serviceProvider.GetRequiredService<FilePanDbSetting>();
                if (setting == null)
                    throw new ArgumentNullException(nameof(setting));

                var connectionName = typeof(CloudSpeedDbContext).Name;
                UseDbServerType(options, connectionName, setting);
            });
            services.AddDbContext<MemberDbContext>((serviceProvider, options) =>
            {
                var setting = serviceProvider.GetRequiredService<MemberDbSetting>();
                options.UseSqlite(setting.Connection, q =>
                {
                    q.MigrationsAssembly(setting.MigrationsAssemblyName);
                });
            });
            return services;
        }

        private static void UseDbServerType(DbContextOptionsBuilder options, string connectionName, IConnSetting connSetting)
        {
            var connectionString = connSetting.Connection;
            var migrationsAssemblyName = connSetting.MigrationsAssemblyName;
            switch (connSetting.DbServer)
            {
                case DbServerType.Sqlite:
                    options.UseSqlite(connectionString, q =>
                    {
                        if (!string.IsNullOrEmpty(migrationsAssemblyName))
                        {
                            q.MigrationsAssembly(migrationsAssemblyName);
                        }
                    });
                    break;
                case DbServerType.InMemory:
                    options.UseInMemoryDatabase(databaseName: connectionName);
                    break;
            }
        }
    }
}
