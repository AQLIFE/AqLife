using AqLife.Domain.Entities;
using Core.Search;

namespace Category.Search;

public sealed class FilteredCategorySearchStrategy
    : FilteredSearchStrategyBase<CategoryEntity, EntitySearchCriteria>
{
    protected override IQueryable<CategoryEntity> ApplyKeywordFilter(IQueryable<CategoryEntity> queryable, string keyword)
        => queryable.Where(e => e.CategoryName.Contains(keyword));

    //public override IQueryable<CategoryEntity> Execute(IQueryable<CategoryEntity> queryable, EntitySearchCriteria criteria, CancellationToken ct = default)
    //    => criteria.UID is Guid uid ? queryable.Where(category => category.UID == uid) : queryable;
}
