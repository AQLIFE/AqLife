using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class CreateTodoCommandHandler(IApplicationDbContext db):IRequestHandler<CreateTodoCommand,Guid>{
    public async Task<Guid> Handle(CreateTodoCommand q,CancellationToken ct){
        var t=new TodoEntity(q.PlanId,q.EffectiveTitle,q.Description,q.Priority,q.EffectiveParentId,q.SortOrder);
        await db.Todo.AddAsync(t,ct);
        return t.UID;
    }
}
