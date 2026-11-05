using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class RestoreTodoCommandHandler(IApplicationDbContext db):IRequestHandler<RestoreTodoCommand,Guid>{
    public async Task<Guid> Handle(RestoreTodoCommand q,CancellationToken ct){
        var t=await db.Todo.FirstAsync(x=>x.UID==q.UID,ct);
        t.Restore();
        return t.UID;
    }
}
