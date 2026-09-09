using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using AqLife.Application.Business.Corpus;

namespace AqLife.Application.Business.Corpus.Handler
{
    public class CreateCorpusCommandHandler(CorpusMapper mapper, IApplicationDbContext storage) : IRequestHandler<CreateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = mapper.ToEntity(command);
            await storage.Corpus.AddAsync(corpus, ct);
            return corpus.UID;
        }
    }
}
