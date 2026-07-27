using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mapper;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.IView;
using MyLife.Shared.Tools;

namespace MyLife.Application.Business.File.Search;

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
    AppStorage storage, IHttpContextAccessor httpContext,
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
