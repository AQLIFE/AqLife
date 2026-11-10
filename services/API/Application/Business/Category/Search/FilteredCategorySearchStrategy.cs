using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Category.Search;

public sealed class FilteredCategorySearchStrategy
    : FilteredSearchStrategyBase<CategoryEntity, EntitySearchCriteria>
{
    protected private override IQueryable<CategoryEntity> ApplyKeywordFilter(IQueryable<CategoryEntity> queryable, string keyword)
        => queryable.Where(e => e.CategoryName.Contains(keyword));

    public override IQueryable<CategoryEntity> Execute(IQueryable<CategoryEntity> queryable,EntitySearchCriteria criteria,CancellationToken ct = default)
        => criteria.UID is Guid uid ? queryable.Where(category => category.UID == uid) : queryable;
}
