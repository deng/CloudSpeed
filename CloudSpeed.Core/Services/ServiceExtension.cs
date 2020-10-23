using System;
using System.Linq;
using System.Text;
using CloudSpeed.Identity;
using CloudSpeed.Managers;
using CloudSpeed.Powergate;
using CloudSpeed.Repositories;
using CloudSpeed.Sdk;
using CloudSpeed.Settings;
using CloudSpeed.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CloudSpeed.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);
            var loggerFactory = LoggerFactory
                .Create(builder => builder.AddFilter(FilterEntityFrameworkCore).AddLog4Net(configuration.CombineConfigPath("log4net.config")));
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.BindConfiguration(configuration);
            services.AddSingleton<LotusClient>();
            services.UseRepositories();
            services.AddSingleton<IPanPasswordHasher, PanPasswordHasher>();
            services.AddTransient<CloudSpeedManager>();
            services.AddTransient<MemberManager>();
            services.UsePowergateRrpcServices(configuration);
            services.AddMemberIdentity();
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

        public static IServiceCollection AddMemberIdentity(this IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var jwtSetting = GlobalServices.ServiceProvider.GetService<JwtSetting>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting.JwtTokenIssuer,
                    ValidAudience = jwtSetting.JwtTokenAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.JwtTokenKey)),
                    ClockSkew = TimeSpan.FromDays(30)
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                     new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddIdentityCore<Member>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddSignInManager().AddEntityFrameworkStores<MemberDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });
            return services;
        }
    }
}
