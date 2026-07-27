using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Todo.Handler
{
    public class UpdateTodoCommandHandler(IApplicationDbContext storage) : IRequestHandler<UpdateTodoCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTodoCommand command, CancellationToken ct)
        {
            TodoEntity entity = await storage.Todo.FindAsync(command.UID, ct) ?? throw new ResourceNotFoundException("不存在 Todo");
            entity.Update(command.Desc, command.Status, command.Priority);
            return command.UID;
        }
    }
}
