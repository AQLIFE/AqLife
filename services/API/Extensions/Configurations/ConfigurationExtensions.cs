using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Extension.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration AddConfiguration(this IConfiguration configuration, IWebHostEnvironment environment)
        {
            string configFolder = Path.Combine(environment.ContentRootPath, "Configurations");
            
            var configFiles = new[] { "appsettings", "FilePolicy" };
            foreach (string fileName in configFiles)
            {
                string baseConfig = $"{fileName}.json";
                string basePath = Path.Combine(configFolder, baseConfig);

                if (!File.Exists(basePath))
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"核心配置文件不存在{baseConfig}");

                    throw new ConfigurationFileNotFoundException($"核心配置文件不存在: {baseConfig}");
                }
                configuration.AddJsonFile(basePath, optional: true, reloadOnChange: true);

                // 2. 再找环境特定文件 (如 FilePolicy.Development.json) - 设为可选
                var envConfig = $"{fileName}.{environment.EnvironmentName}.json";
                var envPath = Path.Combine(configFolder, envConfig);

                if (File.Exists(envPath))
                {
                    configuration.AddJsonFile(envPath, optional: true, reloadOnChange: true);
                    Log.Debug(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"配置文件已载入{envConfig}");
                }
            }
            configuration.AddEnvironmentVariables();
            return configuration;
        }
    }
}
