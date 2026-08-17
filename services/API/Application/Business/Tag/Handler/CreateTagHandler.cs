using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Tag.Handler
{
    public class CreateTagHandler(IApplicationDbContext storage, TagMapper tagMapper) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = tagMapper.ToEntity(command);
            await storage.Tags.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
