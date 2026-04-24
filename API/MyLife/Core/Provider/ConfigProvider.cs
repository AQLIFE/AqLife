using Microsoft.Extensions.Options;
using MyLife.Core.API;
using MyLife.Core.Define;
using Serilog;

namespace MyLife.Core.Provider
{
    public static class ConfigProvider
    {
        public static WebApplicationBuilder BindConfiguration(this WebApplicationBuilder builder)
        {
            string configFolder = Path.Combine(builder.Environment.ContentRootPath, "Core", "Config");
            builder.Configuration.Sources.Clear();
            var configFiles = new[] { "appsettings", "FilePolicy" };
            foreach (var configFile in configFiles)
            {
                var config = $"{configFile}.{builder.Environment.EnvironmentName}.json";
                var configPath = Path.Combine(configFolder, config);
                if (!File.Exists(configPath))
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "初始化配置文件失败,配置文件不存在!");
                    throw new Exception($"无法读取配置文件: {config}");
                }
                else
                {
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, $"成功加载配置文件: {config}");
                    builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);
                }
            }
            builder.Configuration.AddEnvironmentVariables();
            return builder;
        }
    }
}
