using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Application.Business.File.Abstractions;
using MyLife.Domain.Entities;
using MyLife.Shared.Options;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.File.Search
{
    public class FileSecurityAspect(IOptions<FilePolicyOption> options, IApplicationDbContext storage) : IFileAccessPolicy
    {
        public async Task<IQueryable<FileMetaEntity>> ApplyAccessPolicy(IQueryable<FileMetaEntity> queryable, FileAccessMode mode, bool isAuthenticated)
        {
            return mode switch
            {
                FileAccessMode.Preview => await ApplyPreviewPolicy(queryable, isAuthenticated),
                FileAccessMode.Standard => queryable.Where(e => options.Value.AllowedDownload.Contains(e.Extension)),
                _ => queryable
            };
        }


        // 内部实现预览逻辑（订阅检查等）[cite: 11]
        private async Task<IQueryable<FileMetaEntity>> ApplyPreviewPolicy(IQueryable<FileMetaEntity> queryable, bool isAuthenticated)
        {
            if (isAuthenticated) return queryable;
            AccountEntity author = await storage.Accounts.Include(e => e.Subscriptions).SingleAsync(e => e.IsValid);
            var validGuid = author.Subscriptions.Select(e => e.SubscriptionIcon).ToList();
            validGuid.Add(author.Avatar);
            return queryable.Where(e => validGuid.Contains(e.UID) || options.Value.AllowedDownload.Contains(e.Extension));
        }

    }
}
