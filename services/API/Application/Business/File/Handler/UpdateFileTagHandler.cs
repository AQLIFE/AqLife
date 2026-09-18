using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace AqLife.Application.Business.File.Handler
{
    public class UpdateFileTagHandler(IApplicationDbContext context) : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        {
            // 考虑引入FileSearch,减少手动search; 手动search 容易导致tag 子属性映射失败,从而无法有效清空旧的契约映射,导致重复映射,从而违反唯一约束
            FileMetaEntity entity = await context.File.Include(e=>e.FileTags).ThenInclude(t=>t.Tag).FirstOrDefaultAsync(e=>e.UID==command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");//此时必定鉴权通过
            var tagEntites = await context.Tags.Where(e => command.tags.Contains(e.UID)).ToListAsync();
            context.BlogTags.RemoveRange(entity.FileTags);
            entity.FileTags?.Clear();// 理论清空
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
