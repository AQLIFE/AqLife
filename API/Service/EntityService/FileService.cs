using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Implementations;
using MyLife.Service.MapperService;
using MyLife.Shared.DTOs;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;
using MyLife.Shared.Tools;

namespace MyLife.Service.EntityService
{
    public class FileService(
        FileSearch fileSearch,
        IOptions<FilePolicyOption> policy,
        TagMapper tagMapper,
        AppStorage storage,
        UploadContext upContext,
        FileExtensionContentTypeProvider extProvider)//使用框架内置服务
    {
        // 在 全返回中不显示非md文件信息
        public async Task<IEnumerable<FileMetaEntity>?> TryReadAsync(CancellationToken ct, Guid? UID = null, string? Title = null)
            =>await fileSearch.SearchAsync(ct,UID, Title);
            
        
        //[Obsolete("TryReadAsync 已具有更好实现")]
        //public async Task<IEnumerable<FileMetaEntity?>> TryReadListAsync() => await fileSearch.SearchAsync();

        /// <summary>
        /// 预览&下载 API 服务方法 : 生成文件流,提供给 Controller 层直接返回给客户端, 解耦 IO 逻辑和 Web 层的细节
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="OperateTransactionFailedException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<FileDownloadModel> GetFileInternalAsync(Guid id, CancellationToken ct)
        {
            var fileInfo = await fileSearch.SearchAsync(ct,UID:id,isPrivate:true) is IEnumerable<FileMetaEntity> files && files.Any() ? files.First() : throw new RequestTransactionFailedException("不存在文件记录");

            #region 允许下载后检查文件是否存在
            var fullPath = Path.Combine(policy.Value.StoragePath, fileInfo!.UID + fileInfo.Extension);
            if (!File.Exists(fullPath)) throw new FileNotFoundException("源文件丢失,请联系管理员", fullPath);
            #endregion

            #region 释放下载或预览资源
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var ext = GetFileMimeType(fileInfo.UID + fileInfo.Extension);
            return new FileDownloadModel(stream, ext, fileInfo.FileName);
            #endregion
        }

        public async Task<IEnumerable<Guid>> TryCreateAsync(IEnumerable<IFormFile> files, CancellationToken ct)
        {
            var savedPaths = new List<string>();
            var newFileMetas = new List<FileMetaEntity>();
            var resultUIDs = new List<Guid>();


            // 2. 一次性查出数据库中已存在的 Hash（1次 SQL 请求，解决 N+1 痛点）
            var existingFilesDict = await storage.File
                .Where(e => upContext.FileHashes.Select(e => e.Value).Contains(e.FileHash))
                .ToDictionaryAsync(e => e.FileHash, e => e.UID, ct);

            try
            {
                foreach (var item in upContext.FileHashes)
                {
                    // 3. 在内存字典中快速检索，看是否能秒传
                    if (existingFilesDict.TryGetValue(item.Value, out var existingUID))
                    {
                        resultUIDs.Add(existingUID); // 命中秒传，直接用现成的 UID
                    }
                    else
                    {
                        // 未命中，走物理落盘流程
                        var meta = new FileMetaEntity(item.Key, item.Value);
                        var targetPath = Path.Combine(policy.Value.StoragePath, meta.UID + meta.Extension);

                        await SaveFile(item.Key, targetPath);
                        savedPaths.Add(targetPath);
                        newFileMetas.Add(meta);
                        resultUIDs.Add(meta.UID);

                        // 💡 防止同一批次上传中有两个完全一模一样的新文件，将其动态加入临时字典
                        existingFilesDict[item.Value] = meta.UID;
                    }
                }

                // 4. 只有新文件被安全写入数据库
                if (newFileMetas.Count > 0)
                {
                    await storage.File.AddRangeAsync(newFileMetas, ct);
                    // await storage.SaveChangesAsync(ct);
                }

                return resultUIDs;
            }
            catch
            {
                // 异常回滚：清理产生的物理脏文件
                foreach (var path in savedPaths)
                {
                    if (File.Exists(path)) File.Delete(path);
                }
                throw;
            }
        }

        public async Task<Guid> TryUpdateAsync(Guid guid, IFormFile file, CancellationToken ct)
        {
            var entity = await fileSearch.SearchAsync(ct,guid);//此时必定鉴权通过
            if (entity?.First() is FileMetaEntity fileMeta)
            {
                fileMeta.FileHash = upContext.FileHashes.FirstOrDefault(e => e.Key == file).Value;
                var targetPath = Path.Combine(policy.Value.StoragePath, fileMeta.UID + fileMeta.Extension);
                await SaveFile(file, targetPath);
                return guid;
            }
            else
                throw new FileNotFoundException("不存在的文件,无法更新");//重复逻辑,以防万一  
        }

        public async Task<Guid> TryUpdateAsync(Guid guid, IEnumerable<Guid> tags, CancellationToken ct)
        {
            var entity = await fileSearch.SearchAsync(ct, guid);// 此时必定鉴权通过
            if (entity?.First() is FileMetaEntity fileMeta )
            {
                var tagEntites = await storage.Tags.Where(e => tags.Contains(e.UID)).ToListAsync();
                fileMeta.FileTags?.Clear();
                fileMeta.FileTags ??= new List<FileTagEntity>();

                // b. 建立新的契约映射
                foreach (var tag in tagEntites)
                {
                    fileMeta.FileTags.Add(new FileTagEntity
                    {
                        FileId = guid,
                        TagId = tag.UID,
                        //Tag = tag,
                        //File = fileMeta
                    });
                }

                
                return guid;
            }
            else
                throw new FileNotFoundException("不存在的文件,无法更新");//重复逻辑,以防万一  
        }


        public async Task TryDeleteAsync(IEnumerable<Guid?> ids, CancellationToken ct)
        {
            if (ids is null) return;
            foreach (var id in ids)
            {
                if (id is null) continue;
                var file = await storage.File.FindAsync(id) ?? throw new FileNotFoundException("文件不存在");
                var fullPath = Path.Combine(policy.Value.StoragePath, file.UID + file.Extension);
                if (File.Exists(fullPath)) File.Delete(fullPath);
                storage.File.Remove(file);
            }
        }

        public async Task<int> RemoveUnownedFile()
        {
            // 从 指定目录获取所有文件名称,并对数据库记录进行比对, 删除数据库中没有记录的文件, 避免垃圾文件占用存储空间
            var groupFile = Directory.GetFiles(policy.Value.StoragePath);
            var ownedFiles = await storage.File.Select(f => f.UID + f.Extension).ToListAsync();
            var unownedFiles = groupFile.Where(f => !ownedFiles.Contains(Path.GetFileName(f)));

            foreach (var file in unownedFiles)
                File.Delete(file);

            return unownedFiles.Count();
        }
        private string GetFileMimeType(string filename)
        {
            if (extProvider.TryGetContentType(filename, out var contentType))
                return contentType;
            throw new RequestTransactionFailedException("您请求的数据存在异常,已被拦截,若有疑问,请联系管理员");
        }

        /// <summary>
        /// 保存文件到本地,并在保存过程中计算文件的哈希值, 避免重复读取文件两次 (一次计算哈希, 一次保存), 提高性能
        /// </summary>
        /// <param name="file">源文件</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns>文件的哈希值</returns>
        private static async Task SaveFile(IFormFile file, string targetPath)
        {
            using var fs = new FileStream(targetPath, FileMode.Create);
            await file.CopyToAsync(fs);
        }
    }
}