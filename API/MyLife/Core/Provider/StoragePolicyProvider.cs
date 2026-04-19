using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;
using Serilog.Formatting.Compact;

namespace MyLife.Core.Provider
{
    public enum LogType { DbType, ApiType, FunType, ConfigType }

    //public class StructLogConfig(LogType type,string desc)
    //{
    //    public LogType LType { get; init; } = type;
    //    public string LDesc { get; init; } = desc;

    //}
    public static class StoragePolicyProvider
    {
        public static WebApplicationBuilder BindLogger(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // 设置最小记录级别
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")          // 输出到控制台
                .WriteTo.File(new CompactJsonFormatter(), "logs/db_log-.txt", rollingInterval: RollingInterval.Day)// 写入文件，每天一个新文件
                .CreateLogger();

            builder.Host.UseSerilog();

            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "Serilog 已经开始功能工作了!");
            return builder;
        }

        public static WebApplicationBuilder BindStoragePolicy(this WebApplicationBuilder builder)
        {
            var dbconfig = builder.Configuration.GetSection("DbConfig").Get<DBConfig>() ?? throw new Exception("Not found DBConfig!");

            var version = MySqlServerVersion.Parse(dbconfig?.DbVersion ?? throw new Exception("UnKwon Version!"));


            string connStr = builder.Configuration.GetConnectionString(dbconfig?.DbType is string link ? link : string.Empty) ?? throw new Exception($"Not found Key:{dbconfig?.DbType} ConnectionString!");

            builder.Services.AddDbContext<AppStorage>(p => p.UseMySql(connStr, version));
            return builder;
        }

        public static IHost InitCheckDatabaseConnection(this IHost host)
        {
            // 1. 创建服务作用域 (Scope)
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // 2. 通过 DI 提取 Logger 和 DbContext
            // 建议使用 ILogger<AppStorage>，这样日志里会显示是 AppStorage 相关的错误
            var logger = services.GetRequiredService<ILogger<AppStorage>>();
            var context = services.GetRequiredService<AppStorage>();

            try
            {
                logger.LogInformation(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "正在进行数据库连通性自检...");

                // 3. 执行检查
                if (!context.Database.CanConnect())
                {
                    // 此时 Serilog 会接管这个错误，并写入你配置的文本文件
                    logger.LogCritical(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType,"【致命错误】数据库无法连接！请检查配置或 DB 服务状态。");

                    throw new Exception("CRITICAL: Database is unreachable!");
                }

                logger.LogInformation(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType,"数据库连通性自检通过。");
            }
            catch (Exception ex)
            {
                // 记录详细异常，包括堆栈信息
                logger.LogError(ex, @"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "初始化数据库连接时发生未处理的异常。");
                throw; // 继续抛出，确保程序不会在无数据库的情况下启动
            }

            return host;
        }
    }
    public class DBConfig
    {
        public string DbType { get; set; }
        public string DbVersion { get; set; }
    }

    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options)
    {

    }
}