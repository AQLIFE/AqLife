using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
namespace AqLife.Application.Business.Todo.Handler
{
    public class DeleteTodoCommandHandler(IApplicationDbContext storage) : IRequestHandler<DeleteTodoCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTodoCommand command, CancellationToken ct)
        {
            TodoEntity entity = await storage.Todo.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Todo");
            storage.Todo.Remove(entity);
            return Unit.Value;
        }
    }
}
