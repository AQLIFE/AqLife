namespace AqLife.Application.Abstractions.Contracts.Category;

/// <summary>
/// 由拥有分类引用的业务模块实现。
/// Category 模块通过该契约检查分类是否仍被其他模块使用。
/// </summary>
public interface ICategoryReferenceProvider
{
    Task<bool> IsReferencedAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
}