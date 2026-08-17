using MediatR;
using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;


namespace MyLife.Application.Business.File.Handler
{
    public class UpdateFileTagHandler(IApplicationDbContext context) : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await context.File.FindAsync(command.UID, ct)?? throw new FileNotFoundException("不存在的文件,无法更新");//此时必定鉴权通过
            var tagEntites = await context.Tags.Where(e => command.tags.Contains(e.UID)).ToListAsync();
            context.BlogTags.RemoveRange(entity.FileTags);
            entity.FileTags?.Clear();
            entity.FileTags ??= new List<FileTagEntity>();

            // b. 建立新的契约映射
            foreach (var tag in tagEntites)
            {
                entity.FileTags.Add(new FileTagEntity
                {
                    FileId = command.UID,
                    TagId = tag.UID,
                });
            }
            return command.UID;
        }
    }
}
