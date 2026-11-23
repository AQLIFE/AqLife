using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Corpus;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Corpus.Handler
{
    public class CreateCorpusCommandHandler(ICreateMapper<CorpusEntity, CreateCorpusCommand> mapper, ICorpusPersistence persistence) : IRequestHandler<CreateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = mapper.ToEntity(command);
            await persistence.AddCorpusAsync(corpus, ct);
            return corpus.UID;
        }
    }
}
