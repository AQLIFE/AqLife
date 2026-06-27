using MyLife.Data.Repository;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;
using Serilog;
using Serilog.Formatting.Compact;

namespace MyLife.Web.Extensions
{
    public static class InfrastructureSetup
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
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"核心配置文件不存在{baseConfig}");

                    throw new OptionNotFoundException($"核心配置文件不存在: {baseConfig}");
                }
                builder.Configuration.AddJsonFile(basePath, optional: true, reloadOnChange: true);

                // 2. 再找环境特定文件 (如 FilePolicy.Development.json) - 设为可选
                var envConfig = $"{fileName}.{builder.Environment.EnvironmentName}.json";
                var envPath = Path.Combine(configFolder, envConfig);

                if (File.Exists(envPath))
                {
                    builder.Configuration.AddJsonFile(envPath, optional: true, reloadOnChange: true);
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"配置文件已载入{envConfig}");
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

            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "Serilog 已经开始功能工作了!");
            return builder;
        }

        /// <summary>
        /// 数据库连通性自检
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <exception cref="DataBaseConnectionException"></exception>
        public static IHost InitCheckDatabaseConnection(this IHost host)
        {
            // 1. 创建服务作用域 (Scope)
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // 2. 通过 DI 提取 Logger 和 DbContext
            // 建议使用 ILogger<AppStorage>，这样日志里会显示是 AppStorage 相关的错误
            var logger = services.GetRequiredService<ILogger<AppStorage>>();
            var context = services.GetRequiredService<AppStorage>();

            logger.LogInformation(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "正在进行数据库连通性自检...");
            // 3. 执行检查
            if (!context.Database.CanConnect())
            {
                logger.LogCritical(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "【致命错误】数据库无法连接！请检查配置或 DB 服务状态。");

                throw new DataBaseConnectionException("数据库无法连接！请检查配置或 DB 服务状态。");
            }

            logger.LogInformation(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "数据库连通性自检通过。");

            return host;
        }
    }
}
