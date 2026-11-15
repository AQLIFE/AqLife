using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Business.Tag.Handler
{
    public class CreateTagHandler(IApplicationDbContext storage, ICreateMapper<TagEntity, CreateTagCommand> tagMapper) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = tagMapper.ToEntity(command);
            await storage.Tags.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
