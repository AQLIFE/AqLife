using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Options;
namespace AqLife.Application.File;
/// <summary>查询处于预约状态但尚未发布的博客文章</summary>
public record BlogUnpublishQuery(DateTimeOffset ScheduledTime) : IQuery<IEnumerable<FileDto>>;
public record FileOverviewQuery(PublishStatusSearch PublishStatus = PublishStatusSearch.Default, int RecentCount = 10) : IQuery<FileOverviewDto>;
/// <summary>通用文件查询</summary>
public record FileQuery(Guid? UID = null, string? Title = null, Guid? CategoryUID = null, int Page = 1, int PageSize = 10, FileOrder Order = FileOrder.Latest, FileScope Scope = FileScope.Blog)
    : IPageQuery, IQuery<PageResult<FileDto>>;
