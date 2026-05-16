using MyLife.Shared.Accident;
using MyLife.Shared.Options;
using Serilog;
using Serilog.Formatting.Compact;

namespace MyLife.Web.Policy
{
    public static class OptionPolicy
    {
        /// <summary>
        /// 为程序添加私有配置文件，优先级：核心配置 > 环境特定配置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="OptionNotFoundException">配置文件丢失或无读取权限</exception>
        public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
        {
            string configFolder = Path.Combine(builder.Environment.ContentRootPath, "Configurations");
            builder.Configuration.Sources.Clear();
            var configFiles = new[] { "appsettings", "FilePolicy" };
            foreach (var fileName in configFiles)
            {
                var baseConfig = $"{fileName}.json";
                var basePath = Path.Combine(configFolder, baseConfig);

                if (!File.Exists(basePath))
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, $"核心配置文件不存在{baseConfig}");

                    throw new OptionNotFoundException($"核心配置文件不存在: {baseConfig}");
                }
                builder.Configuration.AddJsonFile(basePath, optional: false, reloadOnChange: true);

                // 2. 再找环境特定文件 (如 FilePolicy.Development.json) - 设为可选
                var envConfig = $"{fileName}.{builder.Environment.EnvironmentName}.json";
                var envPath = Path.Combine(configFolder, envConfig);

                if (File.Exists(envPath))
                {
                    builder.Configuration.AddJsonFile(envPath, optional: true, reloadOnChange: true);
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, $"配置文件已载入{envConfig}");
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
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            string gex = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u1}] {Message:lj}{NewLine}";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: gex)
                .WriteTo.File("logs/db_log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: gex)
                .WriteTo.File(new CompactJsonFormatter(), "logs/db_log-.json", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, "Serilog 已经开始功能工作了!");
            return builder;
        }
    }
}
