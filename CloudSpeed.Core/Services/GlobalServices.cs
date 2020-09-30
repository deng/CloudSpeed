using Autofac;
using System;

namespace CloudSpeed.Services
{
    public static class GlobalServices
    {
        public static IContainer Container { get; set; }

        public static IServiceProvider ServiceProvider { get; set; }
    }
}
