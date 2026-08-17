using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Tag.Handler
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
