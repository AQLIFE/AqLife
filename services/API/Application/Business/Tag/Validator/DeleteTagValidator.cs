using Microsoft.EntityFrameworkCore;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.Tag.Validator
{
    public class DeleteTagValidator(IApplicationDbContext storage) : AbstractValidator<DeleteTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "该 Tag 仍在使用,不允许删除";
        private protected override async Task<bool> IsValidAsync(DeleteTagCommand command, CancellationToken ct)
            => await storage.BlogTags.AnyAsync(e => e.TagId.Equals(command.UID));
    }


}
