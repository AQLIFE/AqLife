using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Data.Repository;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;

namespace MyLife.Data
{
    public static class DataLayerSetup
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var dbconfig = configuration.GetSection(typeof(DbOption).Name).Get<DbOption>() ?? throw new OptionMappingException("无法映射到DB Config");
            // ... 解析版本和连接字符串的逻辑 ...
            var version = MySqlServerVersion.Parse(dbconfig?.DbVersion ?? throw new OptionInvalidException("数据库版本不匹配"));// 暂不支持NET 9以上SDK
            string connStr = configuration.GetConnectionString(dbconfig?.DbType is string link ? link : string.Empty) ?? throw new OptionMappingException($"无法找到 关键字:{dbconfig?.DbType} 连接字符串!");

            services.AddDbContext<AppStorage>(p => p.UseMySql(connStr, version));
            //services.AddHealthChecks().AddMySql(connStr, name: "mysql-check", tags: ["db"]);

            return services;
        }
    }

}