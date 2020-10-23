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

namespace CloudSpeed.Maintainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancelToken = new CancellationTokenSource();
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                cancelToken.Cancel(true);
            };
            var config = AppSettingsService.BuildConfiguration("appsettings.json");
            var services = new ServiceCollection();
            services.AddAppServices(config);
            services.AddSingleton<MaintainManager>();
            var serviceProvider = services.BuildGlobalServices(bulder =>
            {
                bulder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            });
            serviceProvider.GetService<MaintainManager>().Maintain2(cancelToken.Token).Wait();
        }
    }
}
