using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Validator
{
    /// <summary>
    /// Todo 最多允许两层：
    /// 根 Todo (FTID = null) -> 子 Todo (FTID = 根 Todo UID)。
    /// 子 Todo 不能再作为其他 Todo 的父级。
    /// </summary>
    public class CreateTodoValidtor(IApplicationDbContext storage) : AbstractValidator<CreateTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } = "仅允许成为二级待办，当前层级已超限";

        private protected override async Task<bool> IsValidAsync(CreateTodoCommand source, CancellationToken ct)
        {
            // 创建根 Todo，不涉及层级限制。
            if (source.FTID is null)
                return true;

            var parent = await storage.Todo.FindAsync([source.FTID.Value], ct);

            // 只有根 Todo 才可以拥有子 Todo。
            return parent is TodoEntity todo && todo.FTID is null;
        }
    }

    public class CreateTodoFUIDValidtor(IApplicationDbContext storage) : AbstractValidator<CreateTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } = "父待办不存在";

        private protected override async Task<bool> IsValidAsync(CreateTodoCommand source, CancellationToken ct)
        {
            if (source.FTID is null)
                return true;

            return await storage.Todo.AnyAsync(e => e.UID == source.FTID.Value, ct);
        }
    }
}
