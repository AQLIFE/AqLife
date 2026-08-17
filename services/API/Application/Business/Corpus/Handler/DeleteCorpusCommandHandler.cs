using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Application.Abstractions.Persistence;

namespace MyLife.Application.Business.Corpus.Handler
{
    public class DeleteCorpusCommandHandler(IApplicationDbContext storage) : IRequestHandler<DeleteCorepusCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCorepusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await storage.Corpus.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的ID");
            storage.Corpus.Remove(corpus);
            return Unit.Value;
        }
    }
}
