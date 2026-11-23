using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.File;

namespace AqLife.Application.File.Handler
{
    /// <summary>
    /// 更新文件内容
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="fileStorage"></param>
    /// <param name="uploadContext"></param>
    public class UpdateFileHandler(IApplicationDbContext dbContext, IFileStorage fileStorage, UploadContext uploadContext) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await dbContext.File.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            string hash = uploadContext.FileHashes[command.File];
            _ = entity.UpdateHash(hash);
            await fileStorage.SaveAsync(command.File.OpenReadStream(), entity.StorageKey, ct);// 覆写到原来的文件
            entity.IncrementVersion();
            return command.UID;
        }
    }
    /// <summary>
    /// 更新文件标签列表
    /// </summary>
    /// <param name="context"></param>
    public class UpdateFileTagHandler(IApplicationDbContext context) : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        {
            var entity = await context.File.Include(file => file.FileTags).SingleOrDefaultAsync(file => file.UID == command.UID, ct)?? throw new FileNotFoundException("不存在的文件,无法更新");

            var tagIds = (command.tags ?? []).Where(id => id != Guid.Empty).Distinct().ToArray();

            var tags = await context.Tags.Where(tag => tagIds.Contains(tag.UID)).ToListAsync(ct);

            entity.FileTags.Clear();

            entity.FileTags = command.tags?.Select(tagId => new FileTagEntity(entity.UID, tagId)).ToList() ?? [];

            return command.UID;
        }
    }
    /// <summary>
    /// 更新文件分类
    /// </summary>
    /// <param name="dbContext"></param>
    public class UpdateFileCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateFileCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCategoryCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await dbContext.File.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            entity.SetCategory(command.categoryId);
            return command.UID;
        }
    }

    /// <summary>
    /// 若没有指定发布时间，则立即发布；若指定了发布时间，则设置为定时发布
    /// </summary>
    /// <param name="context"></param>
}
