using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Search.Base;

namespace MyLife.Service.Search.File
{
    public class FilteredFilesSearchStrategy : FilteredSearchStrategyBase<FileMetaEntity,FileSearchCriteria>
    {
        protected override IQueryable<FileMetaEntity> ApplyKeywordFilter(
        IQueryable<FileMetaEntity> q, string keyword)
        => q.Where(e => e.FileName.Contains(keyword));
    }
}
