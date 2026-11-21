using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Modules.Tag.Handler
{
    public class CreateTagHandler(IApplicationDbContext dbContext, ICreateMapper<TagEntity, CreateTagCommand> tagMapper) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = tagMapper.ToEntity(command);
            await dbContext.Tags.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
