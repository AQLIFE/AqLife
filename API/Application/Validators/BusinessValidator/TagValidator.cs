using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Command;

namespace MyLife.Application.Validators.BusinessValidator
{
    public class DeleteTagValidator(AppStorage storage) : AbstractValidator<DeleteTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "该 Tag 仍在使用,不允许删除";
        private protected override async Task<bool> IsValidAsync(DeleteTagCommand command, CancellationToken ct)
            => await storage.BlogTags.AnyAsync(e => e.TagId.Equals(command.UID));
    }

    public class CreateTagValidator(AppStorage storage) : AbstractValidator<CreateTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "重复的 Tag 名称";
        private protected override async Task<bool> IsValidAsync(CreateTagCommand command, CancellationToken ct)
            => !await storage.Tags.AnyAsync(e => e.Name == command.Name);
    }
}
