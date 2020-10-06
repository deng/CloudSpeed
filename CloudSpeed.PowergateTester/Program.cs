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
                Console.WriteLine("net id: {0}", addrs.AddrInfo.Id);
                Console.WriteLine("net addrs: {0}", string.Join(',', addrs.AddrInfo.Addrs));
                var botaddrs = powergateClient.Ffs.Addrs(new Ffs.Rpc.AddrsRequest(), powergateClient.BotXFfsToken);
                Console.WriteLine("powergate ffs addrs:");
                Console.WriteLine("ffs addrs: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(botaddrs));
            }
            catch (Exception ex)
            {
                Console.WriteLine("powergate test failed:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
