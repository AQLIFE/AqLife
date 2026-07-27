using MediatR;
using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Application.Business.Todo.Validator
{
    public class DeleteTodoValidator(IApplicationDbContext storage) : AbstractValidator<DeleteTodoCommand>
    {
        private protected override string ErrorMessage { init; get; } = "已完成的待办不允许删除";

        private protected override async Task<bool> IsValidAsync(DeleteTodoCommand source, CancellationToken ct)
        =>! await storage.Todo.AnyAsync(e => e.Status == Domain.Entities.TodoStatus.Completed);
    }
}
