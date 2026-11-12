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
            //IEnumerable<FileMetaEntity> fileList = fileMapper.ToEntity(command);

            if (command.Files.Count() == 0) return [];

            var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, false, ct);// 获取 数据库 和 uploadContext 中都存在的文件哈希对应的文件ID,用于去重

            List<Guid> result = [];
            List<FileMetaEntity> newMetas = [];
            List<string> savedFiles = [];

            try
            {
                foreach (var file in command.Files)
                {
                    CreateFileCommand fileCommand = new CreateFileCommand(file, command.categoryId,null,DateTimeOffset.UtcNow.AddDays(7));
                    var hash = uploadContext.FileHashes[file];

                    if (existingFiles.TryGetValue(hash, out var existingId))
                    {
                        result.Add(existingId);
                        continue;
                    }// 若存在于数据库,则直接将其作为结果返回,不再写入物理文件存储和数据库


                    FileMetaEntity meta = fileMapper.ToEntity(fileCommand).UpdateHash(uploadContext.FileHashes[file]);
                    //await publishService.Handle(meta.UID, publishAt, ct); // 此时数据库不存在对应文件信息
                    if (fileCommand.ScheduledTime is DateTimeOffset offset) meta.PublishMeta.Schedule(offset);
                    else meta.PublishMeta.Publish();

                    await fileStorage.SaveAsync(file.OpenReadStream(), meta.StorageKey, ct);

                    savedFiles.Add(meta.StorageKey);
                    newMetas.Add(meta);
                    result.Add(meta.UID);

                    existingFiles[hash] = meta.UID;
                }

                await appStorage.File.AddRangeAsync(newMetas, ct);

                return result;
            }
            catch
            {
                await RollbackPhysicalFilesAsync(savedFiles, ct);
                throw;
            }
        }

        public async Task<List<Guid>> WriteAsync(CreateFileCommand command, CancellationToken ct)
        {
            var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, command.IsTemplate, ct);// 获取 数据库 和 uploadContext 中都存在的文件哈希对应的文件ID,用于去重

            string savedFiles = string.Empty;

            try
            {
                var hash = uploadContext.FileHashes[command.File];

                if (existingFiles.TryGetValue(hash, out var existingId))
                    return [existingId];
                // 若存在于数据库,则直接将其作为结果返回,不再写入物理文件存储和数据库


                FileMetaEntity meta = fileMapper.ToEntity(command).UpdateHash(uploadContext.FileHashes[command.File]);
                
                if (command.ScheduledTime is DateTimeOffset offset) meta.PublishMeta.Schedule(offset);
                else meta.PublishMeta.Publish();

                await fileStorage.SaveAsync(command.File.OpenReadStream(), meta.StorageKey, ct);
                savedFiles = meta.StorageKey;

                await appStorage.File.AddAsync(meta, ct);


                return [meta.UID];
            }
            catch
            {
                await RollbackPhysicalFilesAsync([savedFiles], ct);
                throw;
            }
        }

        private async Task RollbackPhysicalFilesAsync(List<string> savedPaths, CancellationToken ct)
        {
            foreach (var path in savedPaths)
                await fileStorage.DeleteAsync(path, ct);
        }

        private async Task<Dictionary<string, Guid>> FindExistingFilesAsync(IEnumerable<string> hashes, bool isTemplate, CancellationToken ct)
            => await appStorage.File.Include(e => e.PublishMeta).Where(e => hashes.Contains(e.FileHash) && e.IsTemplate == isTemplate)
                .ToDictionaryAsync(e => e.FileHash, e => e.UID, ct);
    }
}
