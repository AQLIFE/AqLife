using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AqLife.Application.Business.File.Search;

public enum FileAccessMode
{
    Standard, // 默认模式：应用 AllowedDownload 扩展名过滤 [cite: 34]
    [Obsolete("被文件发布机制取代,里程碑式进展,后续不再使用单独的检索逻辑")]
    Preview,  // 预览模式：绕过扩展名检查，应用订阅/头像权限检查 [cite: 11]
    Internal  // 内部模式：全量数据，用于后台管理
}

public readonly record struct FileSearchCriteria(
    Guid? UID,
    string? Keyword,
    Guid? CategoryUID
) : ISearchCriteria;

public class FileSearch(
    QueryMapper queryMapper,
    FileSecurityAspect fileSecurity,
    IApplicationDbContext storage, IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<FileMetaEntity, FileSearchCriteria>> searchStrategies)
    : BaseSearch<FileQuery, FileMetaEntity, FileDto, FileSearchCriteria>(storage, searchStrategies)
{
    private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    protected override FileSearchCriteria MapToCriteria(FileQuery query)
        => queryMapper.ToCriteria(query);
    protected override IQueryable<FileMetaEntity> ApplyDefaultOrder(IQueryable<FileMetaEntity> queryble)
    => queryble.Include(t=>t.PublishMeta).OrderBy(e => e.PublishMeta.PublishAt);
    protected override async Task<IQueryable<FileMetaEntity>> BuildBaseQueryAsync(IQueryable<FileMetaEntity> queryable, FileQuery query)
    {
        queryable = queryable.Include(e=>e.PublishMeta).Include(e=>e.InteractionMeta).Include(e =>e.FileTags).ThenInclude(x => x.Tag);
        FileAccessMode mode = IsValid ? FileAccessMode.Internal : FileAccessMode.Standard;
        return await fileSecurity.ApplyAccessPolicy(queryable, mode);
    }
}
