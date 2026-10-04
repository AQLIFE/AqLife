using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Configuration;

namespace AqLife.Web.Extensions.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder ImportConfiguration(this IConfigurationBuilder configuration, IWebHostEnvironment environment)
        {
            string configFolder = Path.Combine(environment.ContentRootPath, "Configurations");

            var configFiles = new[] { "appsettings", "FilePolicy" };
            foreach (string fileName in configFiles)
            {
                string baseConfig = $"{fileName}.json";

                var envConfig = $"{fileName}.{environment.EnvironmentName}.json";
               configuration = LoadConfigurationFile(configuration, configFolder, baseConfig, envConfig);
            }
            configuration.AddEnvironmentVariables();
            return configuration;
        }
        private static void PathExists(string basePath)
        {
            if (!File.Exists(basePath))
            {
                Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"配置文件不存在{basePath}");

                throw new ConfigurationFileNotFoundException($"核心配置文件不存在: {basePath}");
            }
        }
        private static IConfigurationBuilder LoadConfigurationFile(IConfigurationBuilder configuration, string basePath, params string[] fileNames)
        {
            foreach (string item in fileNames)
            {
                string fullPath = Path.Combine(basePath, item);
                PathExists(fullPath);
                configuration.AddJsonFile(fullPath, optional: true, reloadOnChange: true);
            }
            return configuration;
        }
    }

}
