using CloudSpeed.Powergate;
using CloudSpeed.Services;
using CloudSpeed.Web;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CloudSpeed.PowergateTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = AppSettingsService.BuildConfiguration("appsettings.json");
                var services = new ServiceCollection();
                services.AddAppServices(config);
                var serviceProvider = services.BuildGlobalServices();
                var powergateClient = serviceProvider.GetService<PowergateClient>();
                var addrs = powergateClient.Net.ListenAddr(new Net.Rpc.ListenAddrRequest());
                Console.WriteLine("powergate net info:");
                Console.WriteLine("id: {id}", addrs.AddrInfo.Id);
                Console.WriteLine("addrs: {addrs}", string.Join(',', addrs.AddrInfo.Addrs));
            }
            catch (Exception ex)
            {
                Console.WriteLine("powergate test failed:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
