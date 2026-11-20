using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
namespace AqLife.Application.Modules.Tag.Handler
{
    public class DeleteTagHandler(IApplicationDbContext storage) : IRequestHandler<DeleteTagCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await storage.Tags.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Tag");
            storage.Tags.Remove(entity);
            return Unit.Value;
        }

    }
}
