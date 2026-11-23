using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Tag;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
namespace AqLife.Application.Tag.Handler
{
    public class UpdateTagHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await dbContext.Tags.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Tag");
            entity.Update(command.TagName, command.AliasName);
            return command.UID;
        }
    }
}
