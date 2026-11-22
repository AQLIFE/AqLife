using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using Corpus;
using MediatR;

namespace Corpus.Handler
{
    public class DeleteCorpusCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteCorepusCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCorepusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await dbContext.Corpus.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的ID");
            dbContext.Corpus.Remove(corpus);
            return Unit.Value;
        }
    }
}
