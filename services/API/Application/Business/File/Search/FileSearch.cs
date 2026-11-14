using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;


namespace AqLife.Application.Business.File.Search;


public sealed class FileSearch(
    IOptions<FilePolicyOption> options,
    QueryMapper queryMapper,
    IApplicationDbContext storage, IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<FileMetaEntity, FileSearchCriteria>> searchStrategies)
    : BaseSearch<FileQuery, FileMetaEntity, FileSearchCriteria>(storage, httpContext, searchStrategies)
{
    private static readonly Expression<Func<FileMetaEntity, FileMetaEntity>> AnonymousProjection = e => e.Desensitize();

    /// <summary>
    /// 根据用户身份状态返回不同的查询结果
    /// - 未登录用户仅返回非模板文件且已发布的文件
    /// - 已登录用户返回所有文件,包括模板文件和未发布文件
    /// </summary>
    protected private override IQueryable<FileMetaEntity> ApplyResultPolicy(IQueryable<FileMetaEntity> queryable, FileQuery query)
        => IsValid ? queryable : queryable.Where(e => !e.IsTemplate && e.PublishMeta.PublishStatus == FileStatus.Published).Select(AnonymousProjection);

    protected private override FileSearchCriteria MapToCriteria(FileQuery query)
        => queryMapper.ToCriteria(query);
    protected private override IQueryable<FileMetaEntity> ApplyDefaultOrder(IQueryable<FileMetaEntity> queryble, FileQuery query)
        => query.Order switch
        {

            Shared.Options.FileOrder.MostViewed => queryble.Include(i => i.InteractionMeta).OrderByDescending(e => e.InteractionMeta.ViewCount),
            Shared.Options.FileOrder.Latest => queryble.Include(t => t.PublishMeta).OrderByDescending(e => e.PublishMeta.PublishAt),
            _ => queryble.Include(t => t.PublishMeta).OrderByDescending(e => e.UploadTime)
        };

    protected private override IQueryable<FileMetaEntity> BuildBaseQuery(IQueryable<FileMetaEntity> queryable, FileQuery query)
    {
        queryable = queryable.Include(e => e.PublishMeta).Include(e => e.InteractionMeta).Include(e => e.Category).Include(e => e.FileTags).ThenInclude(x => x.Tag);
        if (query.Scope == FileScope.Template && !IsValid) throw new RequestCheckException("You are not authorized to access template files.");// 前置验证已覆盖，确保未登录用户无法访问模板文件
        return query.Scope switch
        {
            FileScope.Template => queryable.Where(e => e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension)),  // 模板文件仅允许博客类型的模板文件
            FileScope.Image => queryable.Where(e => !e.IsTemplate && options.Value.AllowedImageExtensions.Contains(e.Extension)),   // 图片文件仅允许特定类型的图片文件且非模板
            FileScope.Video => queryable.Where(e => !e.IsTemplate && options.Value.AllowedVideoExtensions.Contains(e.Extension)),   // 视频文件仅允许特定类型的视频文件且非模板
            FileScope.Blog => queryable.Where(e => !e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension)),     // 博客文件仅允许特定类型的博客文件且非模板
            FileScope.Default => queryable.Where(e => !e.IsTemplate && options.Value.AllowedExtensions.Contains(e.Extension)),      // 默认文件仅允许特定类型的文件且非模板
            FileScope.All => queryable.Where(e => options.Value.AllowedExtensions.Contains(e.Extension)),                           // 不区分模板查询全部匹配类型文件
            _ => queryable.Where(e => !e.IsTemplate && options.Value.AllowedBlogExtensions.Contains(e.Extension))                   // 与 FileScope.Blog 相同的默认行为，确保未登录用户无法访问模板文件
        };
    }
}
