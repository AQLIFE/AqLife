using AqLife.Domain.Entities;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Corpus 模块专属持久化入口。
/// </summary>
public interface ICorpusPersistence
{
    IQueryable<CorpusEntity> Corpus { get; }
    Task<CorpusEntity?> FindAsync(Guid uid, CancellationToken cancellationToken = default);
    Task AddCorpusAsync(CorpusEntity corpus, CancellationToken cancellationToken = default);
    void RemoveCorpus(CorpusEntity corpus);
}
