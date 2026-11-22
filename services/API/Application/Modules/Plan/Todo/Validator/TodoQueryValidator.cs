namespace Plan.Todo.Validator;

//public sealed class TodoQueryPlanExistsValidator(IApplicationDbContext dbContext) : AbstractValidator<TodoQuery>
//{
//protected override string ErrorMessage { init; get; } = "开发计划不存在";
//protected override async Task<bool> IsValidAsync(TodoQuery command, CancellationToken ct)
//=> dbContext.DevelopmentPlans.AnyAsync(e=>e.UID == command.)
//protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

//}
