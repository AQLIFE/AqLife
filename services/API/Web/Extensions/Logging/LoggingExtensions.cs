using AqLife.Shared.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;


namespace AqLife.Web.Extensions.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// 配置并启用 Serilog
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder InitialSerilog(this IHostBuilder host)
        {
            Log.Logger = CreateLogger();
            host.UseSerilog();
            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "Serilog 已经开始功能工作了!");
            return host;
        }


        /// <summary>
        /// 构造日志记录方式
        /// </summary>
        /// <returns></returns>
        private static Logger CreateLogger()
        {
            string gex = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u1}] {Message:lj}{NewLine}";
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: gex)
                .WriteTo.File("logs/db_log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: gex)
                .WriteTo.File(new CompactJsonFormatter(), "logs/db_log-.json", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
