using AqLife.Application.Search;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.File.Search
{
    public sealed class FilteredFilesSearchStrategy
        : FilteredSearchStrategyBase<FileMetaEntity, FileSearchCriteria>
    {
        public override bool IsMatch(FileSearchCriteria criteria)
            => criteria.UID is not null ||
               !string.IsNullOrWhiteSpace(criteria.Keyword) ||
               criteria.CategoryUID is not null;

        protected private override IQueryable<FileMetaEntity> ApplyKeywordFilter(
            IQueryable<FileMetaEntity> queryable, string keyword)
            => queryable.Where(e => e.FileName.Contains(keyword));

        public override IQueryable<FileMetaEntity> Execute(
            IQueryable<FileMetaEntity> queryable,
            FileSearchCriteria criteria,
            CancellationToken ct = default)
        {
            return criteria switch
            {
                { UID: { } id } => queryable.Where(e => e.UID == id),
                { CategoryUID: { } categoryId } =>
                    queryable.Where(e => e.CategoryUID == categoryId),
                { Keyword: { Length: > 0 } keyword } =>
                    ApplyKeywordFilter(queryable, keyword.Trim()),
                _ => throw new RequestFailException("不合规的操作，该请求不应被处理")
            };
        }
    }
}
