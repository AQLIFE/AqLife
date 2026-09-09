using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.File.Search
{
    public class FilteredFilesSearchStrategy : FilteredSearchStrategyBase<FileMetaEntity, EntitySearchCriteria>
    {
        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(
        IQueryable<FileMetaEntity> q, string keyword)
        => q.Where(e => e.FileName.Contains(keyword));
    }
}
