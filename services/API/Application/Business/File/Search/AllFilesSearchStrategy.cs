using AqLife.Application.Search;
using AqLife.Domain.Entities.File;

namespace AqLife.Application.Business.File.Search
{
    public sealed class AllFilesSearchStrategy : AllSearchStrategyBase<FileMetaEntity, FileSearchCriteria>
    {
        public override bool IsMatch(FileSearchCriteria c)
        => base.IsMatch(c) && c.CategoryUID is null;
        public override IQueryable<FileMetaEntity> Execute(
            IQueryable<FileMetaEntity> queryable,
            FileSearchCriteria criteria,
            CancellationToken ct = default)
        => queryable;
    }
}
