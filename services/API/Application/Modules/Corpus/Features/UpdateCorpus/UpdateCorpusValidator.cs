using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Corpus;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Corpus.Validator
{
    public class UpdateCorpusValidator(IApplicationDbContext dbContext) : AbstractValidator<UpdateCorpusCommand>
    {
        protected override string ErrorMessage { init; get; } = "语料已存在";
        protected override async Task<bool> IsValidAsync(UpdateCorpusCommand command, CancellationToken ct)
        => !await dbContext.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
