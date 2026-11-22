using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Tag;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Tag.Validator
{

    public class CreateTagValidator(IApplicationDbContext dbContext) : AbstractValidator<CreateTagCommand>
    {
        protected override string ErrorMessage { init; get; } = "重复的 Tag 名称";
        protected override async Task<bool> IsValidAsync(CreateTagCommand command, CancellationToken ct)
            => await dbContext.Tags.AllAsync(e => e.Name != command.Name);
    }
}
