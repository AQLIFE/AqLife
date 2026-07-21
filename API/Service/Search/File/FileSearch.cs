using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Mapper;
using MyLife.Service.Search.Base;
using MyLife.Service.Search.Todo;
using MyLife.Shared.IView;
using MyLife.Shared.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyLife.Service.Search.File;

public enum FileAccessMode
{
    Standard, // 默认模式：应用 AllowedDownload 扩展名过滤 [cite: 34]
    Preview,  // 预览模式：绕过扩展名检查，应用订阅/头像权限检查 [cite: 11]
    Internal  // 内部模式：全量数据，用于后台管理
}
public readonly record struct FileSearchCriteria(
    Guid? UID,
    string? Keyword,
    FileAccessMode Mode = FileAccessMode.Standard // 默认为标准模式
): ISearchCriteria;

public class FileSearch(
    FileSecurityAspect fileSecurity,
    AppStorage storage, IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<FileMetaEntity,FileSearchCriteria>> searchStrategies)
    :BaseSearch<FileQuery, FileMetaEntity, FileDto,FileSearchCriteria>(storage, searchStrategies)
{
    private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    protected override FileSearchCriteria MapToCriteria(FileQuery query)
        => new FileSearchCriteria(query.UID, query.Title, FileAccessMode.Preview);
    protected override async Task<IQueryable<FileMetaEntity>> BuildBaseQueryAsync(IQueryable<FileMetaEntity> queryable, FileQuery query)
    {
        queryable = queryable.Include(e => e.FileTags).ThenInclude(x => x.Tag);
        return await fileSecurity.ApplyAccessPolicy(queryable, FileAccessMode.Preview, IsValid);
    }
}
