using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Infrastructure.Authentication;
using AqLife.Infrastructure.FileStorage;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Infrastructure
{
    public static class DataLayerSetup
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var dbconfig = configuration.GetSection(typeof(DbOption).Name).Get<DbOption>() ?? throw new ConfigurationMappingException("无法映射到DB Config");
            // ... 解析版本和连接字符串的逻辑 ...
            var version = MySqlServerVersion.Parse(dbconfig?.DbVersion ?? throw new ConfigurationValueOutOfRangeException("数据库版本不匹配"));// 暂不支持NET 9以上SDK
            string connStr = configuration.GetConnectionString(dbconfig?.DbType is string link ? link : string.Empty) ?? throw new ConfigurationNotFoundException($"无法找到 关键字:{dbconfig?.DbType} 连接字符串!");

            services.AddDbContext<AppStorage>(p => p.UseMySql(connStr, version));

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContext, AppStorage>();
            services.AddScoped<IFileStorage, LocalFileStorage>();
            services.AddScoped<ITokenProvider<AccountEntity>, TokenProvider>();

            return services;
        }
    }
}
