using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Repository;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.Options;

namespace MyLife.Service.Search.File;

public class FileSearch(AppStorage storage, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options, IEnumerable<ISearchStrategy<FileMetaEntity>> searchStrategies)
{
    private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public async Task<IEnumerable<FileMetaEntity>> SearchAsync(CancellationToken ct, Guid? UID = null, string? Title = null, bool isPrivate = false)
    {
        // 1. 寻找匹配的组策略
        var strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(UID, Title));
        if (strategy == null) return [];

        // 2. 统一收拢数据源的安全切面权限过滤（无跟踪查询）
        IQueryable<FileMetaEntity> queryable = storage.File.Include(e => e.FileTags)?.ThenInclude(x => x.Tag);

        
        // 矛盾点: 
        // 1. 

        if (isPrivate && !IsValid)
        {
            var author = await storage.Account.Include(e => e.Subscriptions).SingleOrDefaultAsync(e => e.IsValid);
            if (author is AccountEntity account)
            {
                var validGuid = account.Subscriptions.Select(e => e.SubscriptionIcon).ToList();
                validGuid.Add(account.Avatar);// 有效返回

                if (validGuid != null)
                    queryable = queryable.Where(e => validGuid.Contains(e.UID) || options.Value.AllowedDownload.Contains(e.Extension));
            }
        }
        else if (!IsValid)
        {
            // 策略判定：只有已登录（IsValid），才能看全量，否则只看 AllowedDownload 和 用户信息 存在关联的文件
            queryable = queryable.Where(e => options.Value.AllowedDownload.Contains(e.Extension));
        }

        // 3. 将过滤后的安全数据源与 Query 交给策略类执行最终编译查询
        return await strategy.ExecuteAsync(queryable, UID, Title);
    }

}
