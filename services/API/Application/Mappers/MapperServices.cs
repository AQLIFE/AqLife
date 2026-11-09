using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Mappers;

/// <summary>
/// SQL 查询阶段已经计算完成的分类视图数据。
/// </summary>
public sealed record CategoryMappingData(
    Guid UID,
    string CategoryName,
    string CategoryDescription,
    int CategoryCount,
    string? ParentName);

/// <summary>
/// 统一处理依赖当前请求身份上下文的视图映射与数据脱敏。
/// 此服务必须按请求作用域注册，不能注册为 Singleton。
/// </summary>
public sealed class MapperServices(
    CategoryMapper categoryMapper,
    IHttpContextAccessor httpContextAccessor,
    IOptions<FilePolicyOption> filePolicy,
    ILogger<MapperServices> logger)
    : IViewMapper<CategoryEntity, CategoryDto>
{
    /// <summary>
    /// 未认证请求只能看到公开业务统计。
    /// 查询 Handler 使用该策略选择 SQL 计数表达式，确保排序和返回值一致。
    /// </summary>
    public bool ShouldDesensitize =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true;

    public CategoryDto ToDto(CategoryEntity source)
    {
        if (!ShouldDesensitize)
        {
            return categoryMapper.ToDto(source);
        }

        var allowedExtensions = filePolicy.Value.AllowedBlogExtensions;
        var categoryCount = source.Files.Count(file =>
            !file.IsTemplate &&
            file.PublishMeta.PublishStatus == FileStatus.Published &&
            allowedExtensions.Contains(file.Extension));

        return new CategoryDto(
            source.UID,
            source.CategoryName,
            source.CategoryDescription,
            categoryCount,
            ParentName: source.ParentCategory?.CategoryName);
    }

    /// <summary>
    /// 将数据库投影结果映射为 API DTO。脱敏策略在查询阶段已应用于 CategoryCount。
    /// </summary>
    public List<CategoryDto> ToDtos(IEnumerable<CategoryMappingData> source)
    {
        var rows = source.ToList();
        var context = httpContextAccessor.HttpContext;
        var visibility = ShouldDesensitize ? "Public" : "Authenticated";

        logger.LogDebug(
            "Mapped {CategoryCount} category rows with {Visibility} visibility. RequestId: {RequestId}",
            rows.Count,
            visibility,
            context?.TraceIdentifier ?? "NoHttpContext");

        return rows
            .Select(item => new CategoryDto(
                item.UID,
                item.CategoryName,
                item.CategoryDescription,
                item.CategoryCount,
                item.ParentName))
            .ToList();
    }
}
