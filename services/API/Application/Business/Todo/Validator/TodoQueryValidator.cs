using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

//public sealed class TodoQueryPlanExistsValidator(IApplicationDbContext dbContext) : AbstractValidator<TodoQuery>
//{
    //private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    //private protected override async Task<bool> IsValidAsync(TodoQuery command, CancellationToken ct)
    //=> dbContext.DevelopmentPlans.AnyAsync(e=>e.UID == command.)
    //private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

//}
