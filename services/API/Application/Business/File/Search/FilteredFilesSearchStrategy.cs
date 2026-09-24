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
    public class FilteredFilesSearchStrategy(IHttpContextAccessor httpContext) : FilteredSearchStrategyBase<FileMetaEntity, EntitySearchCriteria>
    {
        private bool IsAuth = httpContext.HttpContext?.User.Identity?.IsAuthenticated ??false;
        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(
        IQueryable<FileMetaEntity> q, string keyword)
        {
            return IsAuth?q.Where(e=>e.FileName.Contains(keyword)) :
            q.Where(e => e.FileName.Contains(keyword));
        }

        public override async Task<IQueryable<FileMetaEntity>> ExecuteAsync(
           IQueryable<FileMetaEntity> queryable,
           EntitySearchCriteria c,
           CancellationToken ct = default)
        {
            if (c.UID is Guid id)
                return queryable.Where(e => e.UID == id);
            else if (!string.IsNullOrWhiteSpace(c.Keyword))
                return ApplyKeywordFilter(queryable, c.Keyword.Trim());
            else throw new RequestFailException("不合规的操作，该请求不应被处理");
        }
    }
}
