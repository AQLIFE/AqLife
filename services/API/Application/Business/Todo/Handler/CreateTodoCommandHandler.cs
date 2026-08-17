using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Todo.Handler
{
    public class CreateTodoCommandHandler(IApplicationDbContext storage, TodoMapper mapper) : IRequestHandler<CreateTodoCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken ct)
        {
            TodoEntity entity = mapper.ToEntity(command);
            await storage.Todo.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
