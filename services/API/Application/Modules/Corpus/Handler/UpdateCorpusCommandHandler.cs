using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace Corpus.Handler
{
    public class UpdateCorpusCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await dbContext.Corpus.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的ID");
            corpus.ChangeCorpus(command.Content);
            return command.UID;
        }
    }
}
