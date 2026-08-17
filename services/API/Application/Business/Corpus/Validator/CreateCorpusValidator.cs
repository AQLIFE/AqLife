using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Application.Business.Corpus.Validator
{
    public class CreateCorpusValidator(IApplicationDbContext dbContext) :AbstractValidator<CreateCorpusCommand>
    {
        private protected override string ErrorMessage { init; get; } = "语料已存在";
        private protected override async Task<bool> IsValidAsync(CreateCorpusCommand command, CancellationToken ct)
        => !await dbContext.Corpus.AsNoTracking().AnyAsync(a => a.CorpusContent == command.Content);
    }
}
