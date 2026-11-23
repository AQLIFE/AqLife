using AqLife.Application.Abstractions.Contracts.Category;
using AqLife.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.File.Service;

/// <summary>
/// File 模块拥有的分类引用检查。
/// </summary>
public sealed class CategoryFileReferenceProvider(IApplicationDbContext dbContext)
    : ICategoryReferenceProvider
{
    public Task<bool> IsReferencedAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
        => dbContext.File.AnyAsync(
            file => file.CategoryUID == categoryId,
            cancellationToken);
}