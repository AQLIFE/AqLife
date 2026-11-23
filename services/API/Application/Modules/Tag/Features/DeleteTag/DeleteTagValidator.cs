using AqLife.Application.Abstractions.Contracts.Tag;
using AqLife.Application.Tag;
using Core.Validators;

namespace AqLife.Application.Tag.Validator
{
    public class DeleteTagValidator(IEnumerable<ITagReferenceProvider> referenceProviders)
        : AbstractValidator<DeleteTagCommand>
    {
        protected override string ErrorMessage { init; get; } = "该 Tag 仍在使用,不允许删除";

        protected override async Task<bool> IsValidAsync(DeleteTagCommand command, CancellationToken ct)
        {
            foreach (var provider in referenceProviders)
            {
                if (await provider.IsReferencedAsync(command.UID, ct))
                    return false;
            }
            return true;
        }
    }
}
