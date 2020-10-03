using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CloudSpeed.Powergate
{
    public static class ServiceExtension
    {
        public static IServiceCollection UsePowergateRrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            var setting = configuration.GetSection(nameof(PowergateSetting)).Get<PowergateSetting>();

            if (setting == null || string.IsNullOrEmpty(setting.Url))
                throw new ApplicationException("please setup /PowergateSetting at appsettings.json");

            services.AddGrpcClient<Index.Ask.Rpc.RPCService.RPCServiceClient>("IndexAskRPCServiceClient", options =>
             {
                 options.Address = new Uri(setting.Url);
             });
            services.AddGrpcClient<Index.Faults.Rpc.RPCService.RPCServiceClient>("IndexFaultsRPCServiceClient", options =>
            {
                options.Address = new Uri(setting.Url);
            });
            services.AddGrpcClient<Index.Miner.Rpc.RPCService.RPCServiceClient>("IndexMinerRPCServiceClient", options =>
            {
                options.Address = new Uri(setting.Url);
            });
            services.AddGrpcClient<Ffs.Rpc.RPCService.RPCServiceClient>("FfsRPCServiceClient", options =>
            {
                options.Address = new Uri(setting.Url);
            });
            services.AddGrpcClient<Net.Rpc.RPCService.RPCServiceClient>("NetRPCServiceClient", options =>
            {
                options.Address = new Uri(setting.Url);
            });
            services.AddGrpcClient<Wallet.Rpc.RPCService.RPCServiceClient>("WalletRPCServiceClient", options =>
            {
                options.Address = new Uri(setting.Url);
            });
            services.AddTransient<PowergateClient>();
            return services;
        }
    }
}
