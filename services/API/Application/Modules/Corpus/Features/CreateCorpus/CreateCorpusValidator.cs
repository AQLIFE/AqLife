using AqLife.Application.Abstractions.Persistence.Modules;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Corpus.Validator
{
    public class CreateCorpusValidator(ICorpusPersistence persistence) : AbstractValidator<CreateCorpusCommand>
    {
        protected override string ErrorMessage { init; get; } = "语料已存在";
        protected override async Task<bool> IsValidAsync(CreateCorpusCommand command, CancellationToken ct)
        => !await persistence.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
