using Microsoft.EntityFrameworkCore;
using MyLife.Data.Health;
using MyLife.Data.Repository;
using MyLife.Shared.Config;

namespace MyLife.Data
{
    // MyLife.Data/DependencyInjection.cs
    public static class DataServiceRegistration
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var dbconfig = configuration.GetSection(typeof(DBConfig).Name).Get<DBConfig>() ?? throw new Exception("Not found DBConfig!");
            // ... 解析版本和连接字符串的逻辑 ...
            var version = MySqlServerVersion.Parse(dbconfig?.DbVersion ?? throw new Exception("UnKwon Version!"));// 暂不支持NET 9以上SDK
            string connStr = configuration.GetConnectionString(dbconfig?.DbType is string link ? link : string.Empty) ?? throw new Exception($"Not found Key:{dbconfig?.DbType} ConnectionString!");

            services.AddDbContext<AppStorage>(p => p.UseMySql(connStr, version));

            // 注册 HealthCheck 
            services.AddDbContext<AppStorage>(p => p.UseMySql(connStr, version)).AddScoped<HealthFunc>();
            services.AddHealthChecks().AddMySql(connStr, name: "mysql-check", tags: ["db"]);

            return services;
        }
    }

}