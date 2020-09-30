using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CloudSpeed.Services
{
    public static class AppSettingsService
    {
        private readonly static string _DefaultJsonFile = "appsettings.json";

        public static void AppendConfigurations(IConfigurationBuilder configBuilder, params string[] fileNames)
        {
            var tempConfigBuilder = new ConfigurationBuilder();
            tempConfigBuilder.AddJsonFile(_DefaultJsonFile, true, false);
            var tempConfig = tempConfigBuilder.Build();

            foreach (var fileName in fileNames)
            {
                var configPath = tempConfig.CombineConfigPath(fileName);
                if (!File.Exists(configPath))
                    throw new ArgumentNullException(string.Format("config file not found {0}", configPath));
                configBuilder.AddJsonFile(configPath, true, false);
            }
        }

        public static IConfigurationRoot BuildConfiguration(params string[] fileNames)
        {
            var tempConfigBuilder = new ConfigurationBuilder();
            tempConfigBuilder.AddJsonFile(_DefaultJsonFile, true, false);
            var tempConfig = tempConfigBuilder.Build();

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(_DefaultJsonFile, true, false);
            foreach (var fileName in fileNames)
            {
                var configPath = tempConfig.CombineConfigPath(fileName);
                if (!File.Exists(configPath))
                    throw new ArgumentNullException(string.Format("config file not found {0}", configPath));
                configBuilder.AddJsonFile(configPath, true, false);
            }
            var config = configBuilder.Build();
            return config;
        }

        public static string CombineConfigPath(this IConfiguration config, string fileName)
        {
            var configRoot = config["configRoot"];
            if (string.IsNullOrEmpty(configRoot))
                throw new ArgumentNullException("please setup configRoot");
            var configPath = Path.Combine(configRoot, fileName);
            return configPath;
        }
    }
}
