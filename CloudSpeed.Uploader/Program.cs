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
using Autofac;

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
            var path = args[0];

            var zip = args.Length > 1 ? Boolean.TrueString.ToLower() == args[1].ToLower() : false;
            if (zip)
            {
                Console.WriteLine("zip will be enabled");
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
            var serviceProvider = services.BuildGlobalServices(bulder =>
            {
                bulder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            });
            serviceProvider.GetService<UploadManager>().UploadAll(path, zip, cancelToken.Token).Wait();
        }
    }
}
