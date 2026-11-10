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

    }
}
