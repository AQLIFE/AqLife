using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.Exceptions;

namespace MyLife.Application.Business.Todo.Handler
{
    public class DeleteTodoCommandHandler(AppStorage storage) : IRequestHandler<DeleteTodoCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTodoCommand command, CancellationToken ct)
        {
            TodoEntity entity = await storage.Todo.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Todo");
            storage.Todo.Remove(entity);
            return Unit.Value;
        }
    }
}
