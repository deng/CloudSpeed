using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudSpeed.Powergate
{
    public static class ServiceExtension
    {
        public static IServiceCollection UsePowergateRrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            var setting = configuration.GetSection(nameof(PowergateSetting)).Get<PowergateSetting>();

            if (setting == null || string.IsNullOrEmpty(setting.ServerAddress))
                throw new ApplicationException("please setup /PowergateSetting at appsettings.json");

            var channel = CreateChannel(setting);

            services.AddTransient(sp => new Index.Ask.Rpc.RPCService.RPCServiceClient(channel));
            services.AddTransient(sp => new Index.Faults.Rpc.RPCService.RPCServiceClient(channel));
            services.AddTransient(sp => new Index.Miner.Rpc.RPCService.RPCServiceClient(channel));
            services.AddTransient(sp => new Ffs.Rpc.RPCService.RPCServiceClient(channel));
            services.AddTransient(sp => new Net.Rpc.RPCService.RPCServiceClient(channel));
            services.AddTransient(sp => new Wallet.Rpc.RPCService.RPCServiceClient(channel));

            services.AddTransient<PowergateClient>();
            services.AddSingleton<PowergateSetting>(setting);
            return services;
        }

        private static Channel CreateChannel(PowergateSetting setting)
        {
            var uri = new Uri(setting.ServerAddress);
            var options = new List<ChannelOption>();
            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                if(string.IsNullOrEmpty(setting.RootCertificates) || string.IsNullOrEmpty(setting.ClientKey) || string.IsNullOrEmpty(setting.ClientCert))
                    throw new ApplicationException("please setup /PowergateSetting/RootCertificates,ClientKey,ClientCert at appsettings.json");
                
                string serverCa = File.ReadAllText(setting.RootCertificates);
                string clientKey = File.ReadAllText(setting.ClientKey);
                string clientCert = File.ReadAllText(setting.ClientCert);
                var creds = new SslCredentials(serverCa, new KeyCertificatePair(clientCert, clientKey));
                return new Channel(uri.Host, uri.Port, creds, options);
            }
            else
            {
                return new Channel(uri.Host, uri.Port, ChannelCredentials.Insecure, options);
            }
        }
    }
}
