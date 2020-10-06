using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using CloudSpeed.Services;
using CloudSpeed.Web;
using CloudSpeed.Entities;
using System.IO;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;

namespace CloudSpeed.Uploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("1 args required");
                return;
            }
            var cancelToken = new CancellationTokenSource();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                cancelToken.Cancel(true);
            };

            var config = AppSettingsService.BuildConfiguration("appsettings.json");
            var services = new ServiceCollection();
            services.AddAppServices(config);
            services.AddSingleton<UploadManager>();
            var serviceProvider = services.BuildGlobalServices();
            var path = args[0];
            serviceProvider.GetService<UploadManager>().UploadAll(path, cancelToken.Token).Wait();
        }
    }
}
