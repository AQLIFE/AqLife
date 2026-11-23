using AqLife.Domain.Entities;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Category 模块专属持久化入口。
/// </summary>
public interface ICategoryPersistence
{
    IQueryable<CategoryEntity> Categories { get; }
    Task AddCategoryAsync(CategoryEntity category, CancellationToken cancellationToken = default);
    void RemoveCategory(CategoryEntity category);
}
