using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Shared.Options;

namespace MyLife.Service.Implementations
{
    public class FileSearch(AppStorage storage, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options, IEnumerable<ISearchStrategy> searchStrategies)
    {
        private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        public async Task<IEnumerable<FileMetaEntity>> SearchAsync(Guid? UID = null, string? Title = null, bool isDown = false)
        {
            // 1. 寻找匹配的组策略
            var strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(UID,Title));
            if (strategy == null) return [];

            // 2. 统一收拢数据源的安全切面权限过滤（无跟踪查询）
            IQueryable<FileMetaEntity> queryable = storage.File.AsNoTracking();

            // 策略判定：只有已登录（IsValid）或处于下载模式（isDown），才能看全量，否则只看 AllowedDownload
            // 改为 : 只有已登录（IsValid）/下载模式（isDown）/系统不存在有效账户时，才能看全量
            if (!isDown && !IsValid)
            {
                var author =await storage.Account.FirstOrDefaultAsync(e => e.IsValid);
                var result = author?.Subscriptions.Select(e => e.SubscriptionIcon);
                if(result !=null)
                queryable = queryable.Where(e => options.Value.AllowedDownload.Contains(e.Extension)|| result.Contains(e.UID));

            }

            // 3. 将过滤后的安全数据源与 Query 交给策略类执行最终编译查询
            return await strategy.ExecuteAsync(queryable, UID,Title);
        }
        //public async Task<IEnumerable<FileMetaEntity>> SearchAsync(IResourceExistenceQuery query, bool isDown = false)
    }
}
