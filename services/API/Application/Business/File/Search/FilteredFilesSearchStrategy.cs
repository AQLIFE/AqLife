using AqLife.Application.Search;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.File.Search
{
    public class FilteredFilesSearchStrategy(IHttpContextAccessor httpContext) : FilteredSearchStrategyBase<FileMetaEntity, FileSearchCriteria>
    {
        private bool IsAuth = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        public override bool IsMatch(FileSearchCriteria c)
        => c.UID is not null || !string.IsNullOrWhiteSpace(c.Keyword) || c.CategoryUID is not null;

        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(IQueryable<FileMetaEntity> q, string keyword)
        {
            return q.Where(e => e.FileName.Contains(keyword));
        }

        public override IQueryable<FileMetaEntity> Execute(IQueryable<FileMetaEntity> queryable, FileSearchCriteria c, CancellationToken ct = default)
        {
            return c switch
            {
                { UID: { } id } => queryable.Where(e => e.UID == id),
                { CategoryUID: { } cid } => queryable.Where(e => e.FileTags.Any(ft => ft.TagId == cid)),
                { Keyword: { Length: > 0 } keyword } => ApplyKeywordFilter(queryable, keyword.Trim()),
                _ => throw new RequestFailException("不合规的操作，该请求不应被处理")
            };
        }
    }
}
