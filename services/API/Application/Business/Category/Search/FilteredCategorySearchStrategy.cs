using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Category.Search;

public sealed class FilteredCategorySearchStrategy
    : ISearchStrategy<CategoryEntity, EntitySearchCriteria>
{
    public bool IsMatch(EntitySearchCriteria criteria) => criteria.UID is not null;

    public IQueryable<CategoryEntity> Execute(
        IQueryable<CategoryEntity> queryable,
        EntitySearchCriteria criteria,
        CancellationToken ct = default)
        => criteria.UID is Guid uid
            ? queryable.Where(category => category.UID == uid)
            : queryable;
}
