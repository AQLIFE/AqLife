using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.BackServices;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
namespace AqLife.Application.Business.File.Service
{
    /// <summary>
    /// 同时写入数据库和物理文件存储,如果写入失败,则回滚物理文件存储
    /// </summary>
    /// <param name="appStorage"></param>
    /// <param name="fileStorage"></param>
    /// <param name="uploadContext"></param>
    public class FileWriter(IApplicationDbContext appStorage, IFileStorage fileStorage, UploadContext uploadContext, ICreateMapper<FileMetaEntity, CreateFileCommand> fileMapper)
    {

        public async Task<List<Guid>> WriteAsync(UploadFilesCommand command, CancellationToken ct)
        {
            if (command.Files.Length == 0) return [];
            //var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, false, ct);// 获取 数据库 和 uploadContext 中都存在的文件哈希对应的文件ID,用于去重


            List<Guid> result = [];
            try
            {
                foreach (var file in command.Files)
                {
                    CreateFileCommand fileCommand = new CreateFileCommand(file, command.categoryId, null, DateTimeOffset.UtcNow.AddDays(7));
                    Guid metaId = await this.WriteAsync(fileCommand, ct);
                    result.Add(metaId);
                }

                return result;
            }
            catch
            {
                await RollbackPhysicalFilesAsync(uploadContext.SavePath, ct);
                throw;
            }
        }
        /// <summary>
        /// 写入成功,自动返回 数据ID,并自动写入UploadContext.SavePath
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Guid> WriteAsync(CreateFileCommand command, CancellationToken ct)
        {
            var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, command.IsTemplate, ct);// 获取 数据库 和 uploadContext 中都存在的文件哈希对应的文件ID,用于去重

            var hash = uploadContext.FileHashes[command.File];

            if (existingFiles.TryGetValue(hash, out var existingId))
                return existingId;
            // 若存在于数据库,则直接将其作为结果返回,不再写入物理文件存储和数据库

            FileMetaEntity meta = fileMapper.ToEntity(command).UpdateHash(uploadContext.FileHashes[command.File]);

            if (command.ScheduledTime is DateTimeOffset offset) meta.PublishMeta.Schedule(offset);
            else meta.PublishMeta.Publish();

            _ = await appStorage.File.AddAsync(meta, ct);
            _ = uploadContext.SavePath.Add(meta.StorageKey);
            await fileStorage.SaveAsync(command.File.OpenReadStream(), meta.StorageKey, ct);

            return meta.UID;
        }

        private async Task RollbackPhysicalFilesAsync(HashSet<string> savedPaths, CancellationToken ct)
        {
            foreach (var path in savedPaths)
                await fileStorage.DeleteAsync(path, ct);
        }

        private async Task<Dictionary<string, Guid>> FindExistingFilesAsync(IEnumerable<string> hashes, bool isTemplate, CancellationToken ct)
            => await appStorage.File.Include(e => e.PublishMeta).Where(e => hashes.Contains(e.FileHash) && e.IsTemplate == isTemplate)
                .ToDictionaryAsync(e => e.FileHash, e => e.UID, ct);
    }
}
