using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using AqLife.Application.Business.Tag;

namespace AqLife.Application.Business.Tag.Handler
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
