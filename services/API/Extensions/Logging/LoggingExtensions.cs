using AqLife.Extensions.Infrastructure;
using AqLife.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;

namespace AqLife.Extensions.Logging;

public static class LoggingExtensions
{
    const string outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u1}] {Message:lj}{NewLine}";
    static private Logger DefaultLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(
                "logs/db_log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate)
            .WriteTo.File(
                new CompactJsonFormatter(),
                "logs/db_log-.json",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

    static private Logger TestLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(
                "logs/db_log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate)
            .WriteTo.File(
                new CompactJsonFormatter(),
                "logs/db_log-.json",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    public static IServiceCollection AddAqLifeSerilog(
        this IServiceCollection services,IHostEnvironment environment)
    {
        Log.Logger = environment.IsTest() ? TestLogger : DefaultLogger;

        services.AddSerilog(dispose: true);

        Log.Information(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "Serilog 已经开始工作了!");

        return services;
    }
}
