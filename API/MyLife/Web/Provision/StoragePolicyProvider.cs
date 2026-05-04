using MyLife.Data.Repository;
using MyLife.Shared.Config;

namespace MyLife.Web.Provision
{
    public static class HostExtensions
    {

        /// <summary>
        /// 初始时进行数据库连接检查
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHost InitCheckDatabaseConnection(this IHost host)
        {
            // 1. 创建服务作用域 (Scope)
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // 2. 通过 DI 提取 Logger 和 DbContext
            // 建议使用 ILogger<AppStorage>，这样日志里会显示是 AppStorage 相关的错误
            var logger = services.GetRequiredService<ILogger<AppStorage>>();
            var context = services.GetRequiredService<AppStorage>();

            try
            {
                logger.LogInformation(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "正在进行数据库连通性自检...");

                // 3. 执行检查
                if (!context.Database.CanConnect())
                {
                    // 此时 Serilog 会接管这个错误，并写入你配置的文本文件
                    logger.LogCritical(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "【致命错误】数据库无法连接！请检查配置或 DB 服务状态。");

                    throw new Exception("CRITICAL: Database is unreachable!");
                }

                logger.LogInformation(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "数据库连通性自检通过。");
            }
            catch (Exception ex)
            {
                // 记录详细异常，包括堆栈信息
                logger.LogError(ex, @"[Serilog][{@LogType}]=>{@LogDesc}", LogType.DbType, "初始化数据库连接时发生未处理的异常。");
                throw; // 继续抛出，确保程序不会在无数据库的情况下启动
            }

            return host;
        }
    }
}