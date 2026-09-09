using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Mappers;


namespace AqLife.Application.Business.File.Search;

public enum FileAccessMode
{
    Standard, // 默认模式：应用 AllowedDownload 扩展名过滤 [cite: 34]
    Preview,  // 预览模式：绕过扩展名检查，应用订阅/头像权限检查 [cite: 11]
    Internal  // 内部模式：全量数据，用于后台管理
}
[Obsolete]
public readonly record struct FileSearchCriteria(
    Guid? UID,
    string? Keyword,
    FileAccessMode Mode = FileAccessMode.Standard // 默认为标准模式
) : ISearchCriteria;

public class FileSearch(
    PreviewContext previewContext,
    QueryMapper queryMapper,
    FileSecurityAspect fileSecurity,
    IApplicationDbContext storage, IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<FileMetaEntity, EntitySearchCriteria>> searchStrategies)
    : BaseSearch<FileQuery, FileMetaEntity, FileDto, EntitySearchCriteria>(storage, searchStrategies)
{
    private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    protected override EntitySearchCriteria MapToCriteria(FileQuery query)
        => queryMapper.ToCriteria(query);
    protected override async Task<IQueryable<FileMetaEntity>> BuildBaseQueryAsync(IQueryable<FileMetaEntity> queryable, FileQuery query)
    {
        queryable = queryable.Include(e => e.FileTags).ThenInclude(x => x.Tag);
        FileAccessMode mode = previewContext.IsPreview ? FileAccessMode.Preview : FileAccessMode.Standard;
        if (IsValid) mode = FileAccessMode.Internal;
        return await fileSecurity.ApplyAccessPolicy(queryable, mode, IsValid);
    }
}
