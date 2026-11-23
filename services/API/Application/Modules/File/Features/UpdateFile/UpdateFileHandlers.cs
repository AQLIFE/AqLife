using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence.Modules;
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
    public class UpdateFileHandler(IFilePersistence persistence, IFileStorage fileStorage, UploadContext uploadContext) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await persistence.Files.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
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
    public class UpdateFileTagHandler(IFilePersistence persistence) : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        {
            var entity = await persistence.Files.Include(file => file.FileTags).SingleOrDefaultAsync(file => file.UID == command.UID, ct)?? throw new FileNotFoundException("不存在的文件,无法更新");

            // 标签有效性由前置验证器负责。

            entity.FileTags.Clear();

            entity.FileTags = command.tags?.Select(tagId => new FileTagEntity(entity.UID, tagId)).ToList() ?? [];

            return command.UID;
        }
    }
    /// <summary>
    /// 更新文件分类
    /// </summary>
    /// <param name="dbContext"></param>
    public class UpdateFileCategoryHandler(IFilePersistence persistence) : IRequestHandler<UpdateFileCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCategoryCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await persistence.Files.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            entity.SetCategory(command.categoryId);
            return command.UID;
        }
    }

    /// <summary>
    /// 若没有指定发布时间，则立即发布；若指定了发布时间，则设置为定时发布
    /// </summary>
    /// <param name="context"></param>
}
