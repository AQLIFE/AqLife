using MediatR;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;

namespace MyLife.Application.Business.Corpus.Handler
{
    public class CreateCorpusCommandHandler(CorpusMapper mapper, AppStorage storage) : IRequestHandler<CreateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = mapper.ToEntity(command);
            await storage.Corpus.AddAsync(corpus, ct);
            return corpus.UID;
        }
    }
}
