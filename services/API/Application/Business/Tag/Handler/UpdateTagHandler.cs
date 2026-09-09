using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
namespace AqLife.Application.Business.Tag.Handler
{
    public class UpdateTagHandler(IApplicationDbContext storage) : IRequestHandler<UpdateTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await storage.Tags.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Tag");
            entity.Update(command.TagName, command.IsCategory, command.AliasName);
            return command.UID;
        }
    }
}
