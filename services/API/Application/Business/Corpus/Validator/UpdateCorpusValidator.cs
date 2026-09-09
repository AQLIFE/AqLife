using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Corpus.Validator
{
    public class UpdateCorpusValidator(IApplicationDbContext dbContext) : AbstractValidator<UpdateCorpusCommand>
    {
        private protected override string ErrorMessage { init; get; } = "语料已存在";
        private protected override async Task<bool> IsValidAsync(UpdateCorpusCommand command, CancellationToken ct)
        => !await dbContext.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
