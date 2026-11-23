namespace AqLife.Application.Abstractions.Contracts.Tag;

/// <summary>
/// 由持有 Tag 引用的模块实现，供 Tag 模块检查是否仍可删除。
/// </summary>
public interface ITagReferenceProvider
{
    Task<bool> IsReferencedAsync(Guid tagId, CancellationToken cancellationToken = default);
}
