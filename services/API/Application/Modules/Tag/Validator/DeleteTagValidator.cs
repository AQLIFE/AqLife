using AqLife.Application.Abstractions.Persistence;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
using Tag;

namespace Tag.Validator
{
    public class DeleteTagValidator(IApplicationDbContext dbContext) : AbstractValidator<DeleteTagCommand>
    {
        protected override string ErrorMessage { init; get; } = "该 Tag 仍在使用,不允许删除";

        protected override async Task<bool> IsValidAsync(DeleteTagCommand command, CancellationToken ct)
        {
            var usedByFiles = await dbContext.BlogTags.AnyAsync(e => e.TagId == command.UID, ct);
            if (usedByFiles)
                return false;

            return !await dbContext.FavoriteTags.AnyAsync(e => e.TagId == command.UID, ct);
        }
    }
}
