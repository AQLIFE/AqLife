using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Corpus;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Corpus.Handler
{
    public class UpdateCorpusCommandHandler(ICorpusPersistence persistence) : IRequestHandler<UpdateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await persistence.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在的ID");
            corpus.ChangeCorpus(command.Content);
            return command.UID;
        }
    }
}
