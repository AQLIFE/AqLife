using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.File.Service
{
    public class FileWriter(IApplicationDbContext appStorage, IFileStorage fileStorage, UploadContext uploadContext)
    {
        //public async Task<Guid> WriteAsync(IFormFile file,string hash,CancellationToken ct)
        //{
        //    FileMetaEntity meta = new (file, hash);

        //    await fileStorage.SaveAsync(file.OpenReadStream(), storageFileName, ct);
        //    await appStorage.File.AddAsync(meta, ct);
        //    return meta.UID;
        //}

        public async Task<List<Guid>> WriteAsync(IEnumerable<IFormFile> files, CancellationToken ct, DateTime? publishAt = null)
        {
            List<IFormFile> fileList = files.ToList();

            if (fileList.Count == 0) return [];

            var existingFiles = await FindExistingFilesAsync(uploadContext.FileHashes.Values, ct);

            List<Guid> result = new();
            List<FileMetaEntity> newMetas = new();
            List<string> savedFiles = new();

            try
            {
                foreach (var file in fileList)
                {
                    var hash = uploadContext.FileHashes[file];

                    if (existingFiles.TryGetValue(hash, out var existingId))
                    {
                        result.Add(existingId);
                        continue;
                    }

                    var meta = publishAt is null ? new FileMetaEntity(file, hash) : new FileMetaEntity(file, hash, publishAt.Value);
                    await fileStorage.SaveAsync(file.OpenReadStream(), meta.StorageName, ct);

                    savedFiles.Add(meta.StorageName);

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
            => await appStorage.File.Where(e => hashes.Contains(e.FileHash))
                .ToDictionaryAsync(e => e.FileHash, e => e.UID, ct);
    }
}
