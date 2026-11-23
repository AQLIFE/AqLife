using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Tag;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Tag.Handler
{
    public class DeleteTagHandler(ITagPersistence persistence) : IRequestHandler<DeleteTagCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await persistence.FindAsync(command.UID, ct)
                ?? throw new ResourceNotFoundException("不存在 Tag");
            persistence.RemoveTag(entity);
            return Unit.Value;
        }
    }
}
