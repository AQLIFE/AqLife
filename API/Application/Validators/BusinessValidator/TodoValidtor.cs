using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;

namespace MyLife.Application.Validators.BusinessValidator
{
    public class CreateTodoValidtor(AppStorage storage) : AbstractValidator<CreateTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } ="仅允许成为二级 待办,当前层级已超限";

        private protected override async Task<bool> IsValidAsync(CreateTodoCommand source, CancellationToken ct)
        {
            var entity = await storage.Todo.FindAsync(source.FTID);// 默认必定存在
            if( entity is TodoEntity todo)
            {
                return todo.FTID == null;
            }
            return false;
        }
    }

    public class CreateTodoFUIDValidtor(AppStorage storage) : AbstractValidator<CreateTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } = "父待办不存在";

        private protected override async Task<bool> IsValidAsync(CreateTodoCommand source, CancellationToken ct)
        {
            var entity = await storage.Todo.FindAsync(source.FTID);// 默认必定存在
            
            return entity !=null;
        }
    }


}
