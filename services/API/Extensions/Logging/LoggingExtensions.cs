using AqLife.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;

namespace AqLife.Extensions.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddAqLifeSerilog(
        this IServiceCollection services)
    {
        const string outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u1}] {Message:lj}{NewLine}";

        Log.Logger = new LoggerConfiguration()
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

        services.AddSerilog(dispose: true);

        Log.Information(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "Serilog 已经开始工作了!");

        return services;
    }
}
