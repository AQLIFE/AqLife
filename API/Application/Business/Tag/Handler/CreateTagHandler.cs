using MediatR;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;

namespace MyLife.Application.Business.Tag.Handler
{
    public class CreateTagHandler(AppStorage storage, TagMapper tagMapper) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = tagMapper.ToEntity(command);
            await storage.Tags.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
