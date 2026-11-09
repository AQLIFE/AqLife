using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Category.Search;

public sealed class AllCategorySearchStrategy
    : ISearchStrategy<CategoryEntity, CategorySearchCriteria>
{
    public bool IsMatch(CategorySearchCriteria criteria) => criteria.UID is null;

    public IQueryable<CategoryEntity> Execute(
        IQueryable<CategoryEntity> queryable,
        CategorySearchCriteria criteria,
        CancellationToken ct = default)
        => queryable;
}
