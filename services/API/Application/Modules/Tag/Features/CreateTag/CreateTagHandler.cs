using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Tag;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Tag.Handler
{
    public class CreateTagHandler(ITagPersistence persistence, ICreateMapper<TagEntity, CreateTagCommand> tagMapper) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = tagMapper.ToEntity(command);
            await persistence.AddTagAsync(entity, ct);
            return entity.UID;
        }
    }
}
