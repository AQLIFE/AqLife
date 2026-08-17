using Microsoft.EntityFrameworkCore;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Tag.Validator
{

    public class CreateTagValidator(IApplicationDbContext storage) : AbstractValidator<CreateTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "重复的 Tag 名称";
        private protected override async Task<bool> IsValidAsync(CreateTagCommand command, CancellationToken ct)
            => !await storage.Tags.AnyAsync(e => e.Name == command.Name);
    }
}
