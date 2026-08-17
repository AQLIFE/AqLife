using MyLife.Application.Search;
using MyLife.Domain.Entities;

namespace MyLife.Application.Business.File.Search
{
    public class FilteredFilesSearchStrategy : FilteredSearchStrategyBase<FileMetaEntity, EntitySearchCriteria>
    {
        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(
        IQueryable<FileMetaEntity> q, string keyword)
        => q.Where(e => e.FileName.Contains(keyword));
    }
}
