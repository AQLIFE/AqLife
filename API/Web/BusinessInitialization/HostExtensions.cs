using MyLife.Data.Repository;
using MyLife.Shared.Accident;
using MyLife.Shared.Options;

namespace MyLife.Web.BusinessInitialization
{
    public static class HostExtensions
    {

        /// <summary>
        /// 数据库连通性自检
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <exception cref="DataBaseConnectionException"></exception>
        public static IHost InitCheckDatabaseConnection(this IHost host)
        {
            // 1. 创建服务作用域 (Scope)
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // 2. 通过 DI 提取 Logger 和 DbContext
            // 建议使用 ILogger<AppStorage>，这样日志里会显示是 AppStorage 相关的错误
            var logger = services.GetRequiredService<ILogger<AppStorage>>();
            var context = services.GetRequiredService<AppStorage>();

            logger.LogInformation(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "正在进行数据库连通性自检...");



            // 3. 执行检查
            if (!context.Database.CanConnect())
            {
                logger.LogCritical(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "【致命错误】数据库无法连接！请检查配置或 DB 服务状态。");

                throw new DataBaseConnectionException("数据库无法连接！请检查配置或 DB 服务状态。");
            }

            logger.LogInformation(@"[Serilog][{@BehavioralLevel}]=>{@LogDesc}", BehavioralLevel.DbType, "数据库连通性自检通过。");

            return host;
        }
    }
}