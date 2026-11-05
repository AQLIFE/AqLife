using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class SetTodoParentCommandHandler(IApplicationDbContext db):IRequestHandler<SetTodoParentCommand,Guid>{
    public async Task<Guid> Handle(SetTodoParentCommand q,CancellationToken ct){
        var t=await db.Todo.FirstAsync(x=>x.UID==q.UID,ct);
        if(q.ParentId.HasValue)t.SetParent(q.ParentId);
        else t.RemoveParent();
        return t.UID;
    }
}
