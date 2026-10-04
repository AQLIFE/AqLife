using Amazon.Runtime;
using Amazon.S3;
using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Infrastructure;
using AqLife.Infrastructure.Authentication;
using AqLife.Infrastructure.FileStorage;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AqLife.Extensions.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbConfig =
            configuration
                .GetSection(nameof(DbOption))
                .Get<DbOption>()
            ?? throw new ConfigurationMappingException(
                "无法映射到 DB Config");

        var connectionString =
            configuration.GetConnectionString(dbConfig.DbType);

        services.AddDbContext<AppStorage>(options =>
        {
            switch (dbConfig.DbType)
            {
                case "MySQL":
                    options.UseMySql(
                        connectionString,
                        MySqlServerVersion.Parse(dbConfig.DbVersion),
                        mysql => mysql.MigrationsAssembly(
                            typeof(AppStorage).Assembly.FullName));
                    break;

                case "PostgreSQL":
                    options.UseNpgsql(
                        connectionString,
                        postgres => postgres.MigrationsAssembly(
                            dbConfig.MigrationAssembly));
                    break;

                default:
                    throw new ConfigurationValueOutOfRangeException(
                        $"不支持的数据库类型: {dbConfig.DbType}");
            }
        });

        Log.Debug(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            $"当前使用数据库类型: {dbConfig.DbType}");

        return services;
    }

    public static IHost CheckDatabaseConnection(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<AppStorage>>();
        var context = services.GetRequiredService<AppStorage>();

        logger.LogInformation(
            "[Serilog][{@BehavioralLevel}]=>{@LogDesc}",
            BehavioralLevel.DbType,
            "正在进行数据库连通性自检...");

        if (!context.Database.CanConnect())
        {
            logger.LogCritical(
                "[Serilog][{@BehavioralLevel}]=>{@LogDesc}",
                BehavioralLevel.DbType,
                "【致命错误】数据库无法连接！请检查配置或 DB 服务状态。");

            throw new DataBaseConnectionException(
                "数据库无法连接！请检查配置或 DB 服务状态。");
        }

        logger.LogInformation(
            "[Serilog][{@BehavioralLevel}]=>{@LogDesc}",
            BehavioralLevel.DbType,
            "数据库连通性自检通过。");

        return host;
    }
}
