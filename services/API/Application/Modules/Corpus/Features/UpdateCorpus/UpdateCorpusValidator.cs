using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Corpus;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Corpus.Validator
{
    public class UpdateCorpusValidator(ICorpusPersistence persistence) : AbstractValidator<UpdateCorpusCommand>
    {
        protected override string ErrorMessage { init; get; } = "语料已存在";
        protected override async Task<bool> IsValidAsync(UpdateCorpusCommand command, CancellationToken ct)
        => !await persistence.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
