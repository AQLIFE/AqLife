using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Corpus;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Corpus.Handler
{
    public class DeleteCorpusCommandHandler(ICorpusPersistence persistence) : IRequestHandler<DeleteCorepusCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCorepusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await persistence.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在的ID");
            persistence.RemoveCorpus(corpus);
            return Unit.Value;
        }
    }
}
