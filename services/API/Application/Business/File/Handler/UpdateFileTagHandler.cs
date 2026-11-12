using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Handler
{
    public class UpdateFileTagHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        {
            var entity = await context.File
                .Include(file => file.FileTags)
                .FirstOrDefaultAsync(file => file.UID == command.UID, ct)
                ?? throw new FileNotFoundException("不存在的文件,无法更新");

            var tagIds = (command.tags ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            var tags = await context.Tags
                .Where(tag => tagIds.Contains(tag.UID))
                .ToListAsync(ct);

            entity.FileTags.Clear();

            foreach (var tag in tags)
            {
                entity.FileTags.Add(new FileTagEntity
                {
                    File = entity,
                    Tag = tag
                });
            }

            return command.UID;
        }
    }

    public class UpdateFileTagValidator(IApplicationDbContext context)
        : AbstractValidator<UpdateFileTagCommand>
    {
        private protected override string ErrorMessage { init; get; } = "一个或多个关联标签不存在";

        private protected override async Task<bool> IsValidAsync(UpdateFileTagCommand command, CancellationToken ct)
        {
            var tagIds = (command.tags ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (tagIds.Length == 0)
                return true;

            var count = await context.Tags.CountAsync(tag => tagIds.Contains(tag.UID), ct);
            return count == tagIds.Length;
        }
    }
}
