using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Tag;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
namespace AqLife.Application.Tag.Handler
{
    public class DeleteTagHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteTagCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTagCommand command, CancellationToken ct)
        {
            TagEntity entity = await dbContext.Tags.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Tag");
            dbContext.Tags.Remove(entity);
            return Unit.Value;
        }

    }
}
