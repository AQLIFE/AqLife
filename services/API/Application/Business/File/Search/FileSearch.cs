using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace AqLife.Application.Business.File.Search;

public readonly record struct FileSearchCriteria(
    Guid? UID,
    string? Keyword,
    Guid? CategoryUID
) : ISearchCriteria;

public class FileSearch(
    IOptions<FilePolicyOption> options,
    QueryMapper queryMapper,
    IApplicationDbContext storage, IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<FileMetaEntity, FileSearchCriteria>> searchStrategies)
    : BaseSearch<FileQuery, FileMetaEntity, FileDto, FileSearchCriteria>(storage, searchStrategies)
{
    private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    protected override FileSearchCriteria MapToCriteria(FileQuery query)
        => queryMapper.ToCriteria(query);
    protected override IQueryable<FileMetaEntity> ApplyDefaultOrder(IQueryable<FileMetaEntity> queryble, FileQuery query)
        => query.Order switch
        {

            Shared.Options.FileOrder.MostViewed => queryble.Include(i => i.InteractionMeta).OrderByDescending(e => e.InteractionMeta.ViewCount),
            Shared.Options.FileOrder.Latest => queryble.Include(t => t.PublishMeta).OrderBy(e => e.PublishMeta.PublishAt),
            _ => queryble.Include(t => t.PublishMeta).OrderByDescending(e => e.UploadTime)
        };

    protected override async Task<IQueryable<FileMetaEntity>> BuildBaseQueryAsync(IQueryable<FileMetaEntity> queryable, FileQuery query)
    {
        queryable = queryable.Include(e => e.PublishMeta).Include(e => e.InteractionMeta).Include(e => e.FileTags).ThenInclude(x => x.Tag);
        if(query.Scope == FileScope.Template && !IsValid) throw new RequestCheckException("You are not authorized to access template files.");// 前置验证已覆盖，确保未登录用户无法访问模板文件
        queryable = query.Scope switch
        {
            FileScope.Image => queryable.Where(e => !e.IsTemplate && options.Value.AllowedImageExtensions.Contains(e.Extension)),
            FileScope.Template => queryable.Where(e => e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension)),
            FileScope.Blog => queryable.Where(e => !e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension)),
            _ => IsValid? queryable.Where(e => !e.IsTemplate && options.Value.AllowedExtensions.Contains(e.Extension)):queryable.Where(e=>!e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension))
        };
        return !IsValid?queryable.Where(e => !e.IsTemplate && e.PublishMeta.PublishStatus == FileStatus.Published):queryable;
    }
}
