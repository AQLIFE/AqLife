using MyLife.Shared.Config;
using Serilog;
using Serilog.Formatting.Compact;

namespace MyLife.Web.Provision
{
    public static class ConfigProvider
    {
        public static WebApplicationBuilder BindConfiguration(this WebApplicationBuilder builder)
        {
            string configFolder = Path.Combine(builder.Environment.ContentRootPath, "Web", "Configurations");
            builder.Configuration.Sources.Clear();
            var configFiles = new[] { "appsettings", "FilePolicy" };
            foreach (var fileName in configFiles)
            {
                var baseConfig = $"{fileName}.json";
                var basePath = Path.Combine(configFolder, baseConfig);

                if (!File.Exists(basePath))
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, $"核心配置文件不存在{baseConfig}");
                    throw new Exception($"核心配置文件不存在: {baseConfig}");
                }
                builder.Configuration.AddJsonFile(basePath, optional: false, reloadOnChange: true);

                // 2. 再找环境特定文件 (如 FilePolicy.Development.json) - 设为可选
                var envConfig = $"{fileName}.{builder.Environment.EnvironmentName}.json";
                var envPath = Path.Combine(configFolder, envConfig);

                if (File.Exists(envPath))
                {
                    builder.Configuration.AddJsonFile(envPath, optional: true, reloadOnChange: true);
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, $"配置文件已载入{envConfig}");
                }
            }
            builder.Configuration.AddEnvironmentVariables();
            return builder;
        }

        /// <summary>
        /// 配置并启用 Serilog
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder BindLogger(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u1}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(new CompactJsonFormatter(), "logs/db_log-.json", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "Serilog 已经开始功能工作了!");
            return builder;
        }
    }
}
