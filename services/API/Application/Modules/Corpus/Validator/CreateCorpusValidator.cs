using AqLife.Application.Abstractions.Persistence;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Corpus.Validator
{
    public class CreateCorpusValidator(IApplicationDbContext dbContext) : AbstractValidator<CreateCorpusCommand>
    {
        protected override string ErrorMessage { init; get; } = "语料已存在";
        protected override async Task<bool> IsValidAsync(CreateCorpusCommand command, CancellationToken ct)
        => !await dbContext.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
