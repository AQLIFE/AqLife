using MediatR;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;

namespace MyLife.Application.Business.Todo.Handler
{
    public class CreateTodoCommandHandler(AppStorage storage, TodoMapper mapper) : IRequestHandler<CreateTodoCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken ct)
        {
            TodoEntity entity = mapper.ToEntity(command);
            await storage.Todo.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
