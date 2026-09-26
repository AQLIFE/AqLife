using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.File.Search
{
    public class FilteredFilesSearchStrategy(IHttpContextAccessor httpContext) : FilteredSearchStrategyBase<FileMetaEntity, FileSearchCriteria>
    {
        private bool IsAuth = httpContext.HttpContext?.User.Identity?.IsAuthenticated ??false;
        public override bool IsMatch(FileSearchCriteria c)
        => c.UID is not null || !string.IsNullOrWhiteSpace(c.Keyword) || c.CategoryUID is not null;

        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(
        IQueryable<FileMetaEntity> q, string keyword)
        {
            return IsAuth?q.Where(e=>e.FileName.Contains(keyword)) :
            q.Where(e => e.FileName.Contains(keyword));
        }

        public override async Task<IQueryable<FileMetaEntity>> ExecuteAsync(
           IQueryable<FileMetaEntity> queryable,
           FileSearchCriteria c,
           CancellationToken ct = default)
        {
            if (c.UID is Guid id)
                return queryable.Where(e => e.UID == id);
            else if (!string.IsNullOrWhiteSpace(c.Keyword))
                return ApplyKeywordFilter(queryable, c.Keyword.Trim());
            else if (c.CategoryUID is Guid categoryId)
                return queryable.Where(e => e.FileTags.Any(ft => ft.TagId == categoryId) && e.PublishMeta.PublishStatus == FileStatus.Published );
                    
            else throw new RequestFailException("不合规的操作，该请求不应被处理");
        }
    }
}
