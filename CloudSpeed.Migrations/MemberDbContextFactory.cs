using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using CloudSpeed.Services;
using CloudSpeed.Web;
using CloudSpeed.Identity;

namespace CloudSpeed.Migrations
{
    public class MemberDbContextFactory : IDesignTimeDbContextFactory<MemberDbContext>
    {
        public MemberDbContext CreateDbContext(string[] args)
        {
            var config = AppSettingsService.BuildConfiguration("appsettings.json");
            var services = new ServiceCollection();
            services.AddAppServices(config);
            var serviceProvider = services.BuildGlobalServices();
            return serviceProvider.GetService<MemberDbContext>();
        }
    }
}
