using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Search;

public readonly record struct CategorySearchCriteria(Guid? UID, string? Keyword = null) : ISearchCriteria;

public sealed class CategorySearch(
    IApplicationDbContext dbContext,
    IEnumerable<ISearchStrategy<CategoryEntity, CategorySearchCriteria>> strategies)
{
    public Task<IQueryable<CategoryEntity>> SearchAsync(CategoryQuery query, CancellationToken ct = default)
    {
        var criteria = new CategorySearchCriteria(query.CID);
        var strategy = strategies.FirstOrDefault(item => item.IsMatch(criteria))
            ?? throw new InvalidOperationException("未找到可用的 Category 搜索策略");

        IQueryable<CategoryEntity> source = dbContext.Categories
            .AsNoTracking()
            .Include(category => category.ParentCategory);

        return Task.FromResult(strategy.Execute(source, criteria, ct));
    }
}