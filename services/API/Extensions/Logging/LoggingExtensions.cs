using AqLife.Extensions.Infrastructure;
using AqLife.Shared.Options;
using Microsoft.Extensions.Configuration;
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
    static private LoggerConfiguration BaseLoggerConfig => new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(
                "logs/db_log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate)
            .WriteTo.File(
                new CompactJsonFormatter(),
                "logs/db_log-.json",
                rollingInterval: RollingInterval.Day);
    static private LoggerConfiguration TestLogger => BaseLoggerConfig;
    static private LoggerConfiguration DefaultLogger => BaseLoggerConfig.WriteTo.Console(outputTemplate:outputTemplate);

    public static IServiceCollection AddAqLifeSerilog(
        this IServiceCollection services,IHostEnvironment environment,IConfiguration configuration)
    {
        LoggerConfiguration loggerConfig = environment.IsTest() ? TestLogger : DefaultLogger;

        Log.Logger = loggerConfig.ReadFrom.Configuration(configuration).CreateLogger();

        services.AddSerilog(dispose: true);

        Log.Information("[Serilog][{@LogType}]=>{@LogDesc}",BehavioralLevel.OptionType,"Serilog 已经开始工作了!");

        return services;
    }
}
