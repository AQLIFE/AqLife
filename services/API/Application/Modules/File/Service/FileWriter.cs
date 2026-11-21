using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Tools;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.File.Service
{
    /// <summary>
    /// 同时写入数据库和物理文件存储；如果写入失败，则回滚物理文件存储。
    /// </summary>
    public class FileWriter(
        IApplicationDbContext appStorage,
        AqLife.Application.Abstractions.Contracts.File.Storage.IFileStorage fileStorage,
        UploadContext uploadContext,
        ICreateMapper<FileMetaEntity, CreateFileCommand> fileMapper)
    {
        public async Task<List<Guid>> WriteAsync(UploadFilesCommand command, CancellationToken ct)
        {
            if (command.Files.Length == 0) return [];

            List<Guid> result = [];
            try
            {
                foreach (var file in command.Files)
                {
                    CreateFileCommand fileCommand = new CreateFileCommand(
                        file,
                        command.categoryId,
                        null,
                        DateTimeOffset.UtcNow.AddDays(7));

                    Guid metaId = await WriteAsync(fileCommand, ct);
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
        /// 写入成功后返回数据 ID，并将物理路径登记到 UploadContext。
        /// </summary>
        public async Task<Guid> WriteAsync(CreateFileCommand command, CancellationToken ct)
        {
            var hash = await uploadContext.GetOrAddFileHashAsync(command.File, ct);
            var existingFiles = await FindExistingFilesAsync(
                uploadContext.FileHashes.Values,
                command.IsTemplate,
                ct);

            if (existingFiles.TryGetValue(hash, out var existingId))
                return existingId;

            FileMetaEntity meta = fileMapper.ToEntity(command)
                .UpdateHash(hash)
                .SetCategory(command.CategoryId);

            if (command.ScheduledTime is DateTimeOffset offset)
                meta.PublishMeta.Schedule(offset);
            else
                meta.PublishMeta.Publish();

            await appStorage.File.AddAsync(meta, ct);
            uploadContext.SavePath.Add(meta.StorageKey);

            await using var content = command.File.OpenReadStream();
            await fileStorage.SaveAsync(content, meta.StorageKey, ct);

            return meta.UID;
        }

        private async Task RollbackPhysicalFilesAsync(HashSet<string> savedPaths, CancellationToken ct)
        {
            foreach (var path in savedPaths)
                await fileStorage.DeleteAsync(path, ct);
        }

        private async Task<Dictionary<string, Guid>> FindExistingFilesAsync(
            IEnumerable<string> hashes,
            bool isTemplate,
            CancellationToken ct)
            => await appStorage.File
                .Where(file => hashes.Contains(file.FileHash) && file.IsTemplate == isTemplate)
                .ToDictionaryAsync(file => file.FileHash, file => file.UID, ct);
    }
}
