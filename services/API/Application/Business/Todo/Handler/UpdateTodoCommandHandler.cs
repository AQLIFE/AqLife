using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
namespace AqLife.Application.Business.Todo.Handler
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
