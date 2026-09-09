using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Tag.Validator
{

    public class CreateTagValidator(IApplicationDbContext storage) : AbstractValidator<CreateTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "重复的 Tag 名称";
        private protected override async Task<bool> IsValidAsync(CreateTagCommand command, CancellationToken ct)
            => !await storage.Tags.AnyAsync(e => e.Name == command.Name);
    }
}
