using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Services;
using CloudSpeed.Web;
using CloudSpeed.Entities;

namespace CloudSpeed.Migrations
{
    public class CloudSpeedDbContextFactory : IDesignTimeDbContextFactory<CloudSpeedDbContext>
    {
        public CloudSpeedDbContext CreateDbContext(string[] args)
        {
            var config = AppSettingsService.BuildConfiguration("appsettings.json");
            var services = new ServiceCollection();
            services.AddAppServices(config);
            var serviceProvider = services.BuildGlobalServices();
            return serviceProvider.GetService<CloudSpeedDbContext>();
        }
    }
}
