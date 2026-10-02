using Amazon.Runtime;
using Amazon.S3;
using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Infrastructure.Authentication;
using AqLife.Infrastructure.FileStorage;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace AqLife.Infrastructure
{
    public static class DataLayerSetup
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            DbOption dbconfig = configuration.GetSection(typeof(DbOption).Name).Get<DbOption>() ?? throw new ConfigurationMappingException("无法映射到DB Config");

            var connStr = configuration.GetConnectionString(dbconfig.DbType);

            services.AddDbContext<AppStorage>(p =>
            {
                switch (dbconfig.DbType)
                {
                    case "MySQL":
                        p.UseMySql(connStr, MySqlServerVersion.Parse(dbconfig?.DbVersion), mysql => mysql.MigrationsAssembly(
                typeof(AppStorage).Assembly.FullName));
                        break;
                    case "PostgreSQL":
                        p.UseNpgsql(connStr, postgres => postgres.MigrationsAssembly(dbconfig.MigrationAssembly));
                        break;
                    default: throw new ConfigurationValueOutOfRangeException($"不支持的数据库类型: {dbconfig.DbType}");
                }

            });
            Log.Debug(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"当前使用数据库类型:{dbconfig.DbType}");

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IHostEnvironment environment)
        {
            services.AddScoped<IApplicationDbContext, AppStorage>();
           
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var options = sp
                    .GetRequiredService<IOptions<R2Options>>()
                    .Value;

                var credentials = new BasicAWSCredentials(
                    options.AccessKey,
                    options.SecretKey);

                return new AmazonS3Client(
                    credentials,
                    new AmazonS3Config
                    {
                        ServiceURL = options.Endpoint,
                        ForcePathStyle = true
                    });
            });
            services.AddScoped<IFileStorage, R2FileStorage>();
            Log.Debug(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "当前使用默认存储方案: Cloudflare R2");
            services.AddScoped<ITokenProvider<AccountEntity>, TokenProvider>();

            return services;
        }
    }
}
