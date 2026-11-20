using AqLife.Application.Abstractions.Persistence;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Tag.Validator
{
    public class DeleteTagValidator(IApplicationDbContext storage) : AbstractValidator<DeleteTagCommand>
    {
        protected override string ErrorMessage { init; get; } = "该 Tag 仍在使用,不允许删除";

        protected override async Task<bool> IsValidAsync(DeleteTagCommand command, CancellationToken ct)
        {
            var usedByFiles = await storage.BlogTags.AnyAsync(e => e.TagId == command.UID, ct);
            if (usedByFiles)
                return false;

            return !await storage.FavoriteTags.AnyAsync(e => e.TagId == command.UID, ct);
        }
    }
}
