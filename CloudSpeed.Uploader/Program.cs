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
using CommandLine;

namespace CloudSpeed.Uploader
{
    class Program
    {
        public class Options
        {
            [Option('p', "path", Required = true, HelpText = "The path to a file or directory that is to be imported.")]
            public string Path { get; set; }

            [Option('z', "z", Required = false, HelpText = "Compress the files.")]
            public bool Zip { get; set; }
        }

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
            var serviceProvider = services.BuildGlobalServices();
            serviceProvider.GetService<UploadManager>().UploadAll(path, zip, cancelToken.Token).Wait();
        }
    }
}
