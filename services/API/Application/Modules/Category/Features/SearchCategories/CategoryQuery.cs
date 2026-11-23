using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;

namespace AqLife.Application.Category;

public enum CategoryQueryType
{
    Default,
    File,
    Favorite
}

/// <summary>
/// 查询分类及其关联业务数量。UID 指定时仅返回对应分类。
/// </summary>
public record CategoryQuery(Guid? UID = null, CategoryQueryType SearchType = CategoryQueryType.File, int Page = 1, int PageSize = 10)
    : IQuery<PageResult<CategoryDto>>, IPageQuery;
