using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Tag;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Tag.Handler
{
    public class UpdateTagHandler(ITagPersistence persistence) : IRequestHandler<UpdateTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await persistence.FindAsync(command.UID, ct)
                ?? throw new ResourceNotFoundException("不存在 Tag");
            entity.Update(command.TagName, command.AliasName);
            return command.UID;
        }
    }
}
