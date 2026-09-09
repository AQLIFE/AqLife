using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Validator
{
    public class DeleteTodoValidator(IApplicationDbContext storage) : AbstractValidator<DeleteTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } = "已完成的待办不允许删除";

        private protected override async Task<bool> IsValidAsync(DeleteTodoCommand source, CancellationToken ct)
        => !await storage.Todo.AnyAsync(e => e.Status == TodoStatus.Completed);
    }
}
