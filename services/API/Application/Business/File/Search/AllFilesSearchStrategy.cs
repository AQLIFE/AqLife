using AqLife.Application.Search;
using AqLife.Domain.Entities.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AqLife.Application.Business.File.Search
{
    public sealed class AllFilesSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<FileMetaEntity, FileSearchCriteria>(httpContext)
    {
        public override bool IsMatch(FileSearchCriteria c)
        => base.IsMatch(c) && c.CategoryUID is null;
        public override IQueryable<FileMetaEntity> Execute(
            IQueryable<FileMetaEntity> queryable,
            FileSearchCriteria criteria,
            CancellationToken ct = default)
        => base.IsValid? queryable:queryable.Select(e=>e.Desensitize());// 仅在查询中使用,不允许在事务中使用;这样就可以取消 FileMappingService 
    }
}
