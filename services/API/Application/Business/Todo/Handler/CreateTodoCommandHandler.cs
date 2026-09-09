using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using AqLife.Application.Business.Todo;

namespace AqLife.Application.Business.Todo.Handler
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
