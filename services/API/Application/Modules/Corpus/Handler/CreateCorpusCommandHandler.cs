using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Modules.Corpus.Handler
{
    public class CreateCorpusCommandHandler(ICreateMapper<CorpusEntity, CreateCorpusCommand> mapper, IApplicationDbContext dbContext) : IRequestHandler<CreateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = mapper.ToEntity(command);
            await dbContext.Corpus.AddAsync(corpus, ct);
            return corpus.UID;
        }
    }
}
