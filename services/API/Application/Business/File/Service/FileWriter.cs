using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.File.Service
{
    /// <summary>
    /// 同时写入数据库和物理文件存储,如果写入失败,则回滚物理文件存储
    /// </summary>
    /// <param name="appStorage"></param>
    /// <param name="fileStorage"></param>
    /// <param name="uploadContext"></param>
    public class FileWriter(IApplicationDbContext appStorage, IFileStorage fileStorage, UploadContext uploadContext,ICreateMapper<FileMetaEntity,IFormFile> fileMapper)
    {

        public async Task<List<Guid>> WriteAsync(IEnumerable<IFormFile> files, CancellationToken ct, DateTimeOffset? publishAt = null)
        {
            List<IFormFile> fileList = files.ToList();

            if (fileList.Count == 0) return [];

            var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, ct);// 获取 数据库 和 uploadContext 中都存在的文件哈希对应的文件ID,用于去重

            List<Guid> result = [];
            List<FileMetaEntity> newMetas = [];
            List<string> savedFiles = [];

            try
            {
                foreach (var file in fileList)
                {
                    var hash = uploadContext.FileHashes[file];

                    if (existingFiles.TryGetValue(hash, out var existingId))
                    {
                        result.Add(existingId);
                        continue;
                    }// 若存在于数据库,则直接将其作为结果返回,不再写入物理文件存储和数据库

                    
                    FileMetaEntity meta = fileMapper.ToEntity(file);
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

        private async Task RollbackPhysicalFilesAsync(List<string> savedPaths, CancellationToken ct)
        {
            foreach (var path in savedPaths)
                await fileStorage.DeleteAsync(path, ct);

        }

        private async Task<Dictionary<string, Guid>> FindExistingFilesAsync(IEnumerable<string> hashes, CancellationToken ct)
            => await appStorage.File.Include(e=>e.PublishMeta).Where(e => hashes.Contains(e.FileHash))
                .ToDictionaryAsync(e => e.FileHash, e => e.UID, ct);
    }
}
