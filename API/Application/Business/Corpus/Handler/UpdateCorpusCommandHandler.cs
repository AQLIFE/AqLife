using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Application.Abstractions.Persistence;

namespace MyLife.Application.Business.Corpus.Handler
{
    public class UpdateCorpusCommandHandler(IApplicationDbContext storage) : IRequestHandler<UpdateCorpusCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateCorpusCommand command, CancellationToken ct)
        {
            CorpusEntity corpus = await storage.Corpus.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的ID");
            corpus.CorpusContent = command.Content;// EF Coee 会自动跟踪实体变化
            return command.UID;
        }
    }
}
