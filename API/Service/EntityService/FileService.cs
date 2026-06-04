using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Service.StrategiesService;
using MyLife.Shared.Accident;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyLife.Service.EntityService
{
    public class FileService(
        IEnumerable<IUploadStrategy> uploadStrategies,
        IUploadStrategyAsync effectivenessCheck,
        IOptions<FilePolicyOption> policy,
        AppStorage storage,
        UploadContext upContext,
        FileExtensionContentTypeProvider _extProvider,
        IHttpContextAccessor httpContext)//使用框架内置服务
    {
        private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public async Task<IEnumerable<FileMetaEntity?>> TryReadAsync(Guid? guid, string? title)
        {
            var query = storage.File.AsQueryable();

            //if (!IsValid)
            //{ 过滤导致仅有mD文件可被检索
            //    var allowedExtensions = policy.Value.AllowedDownload.Select(ext => ext.ToLowerInvariant()).ToList();
            //    query = query.Where(e => allowedExtensions.Contains(e.Extension));
            //}

            if (guid.HasValue)
                query = query.Where(e => e.UID == guid.Value);
            else if (title is not null && !string.IsNullOrEmpty(title))
                query = query.Where(e => e.FileName.Contains(title));
            else return null;
            return query.AsEnumerable();
        }

        private async Task<IEnumerable<FileMetaEntity?>> PublicListAsync()
        {
            var allowedExtensions = policy.Value.AllowedDownload.Select(ext => ext.ToLowerInvariant()).ToList();
            return await storage.File.AsNoTracking().Where(e => allowedExtensions.Contains(e.Extension)).OrderBy(e => e.FileName).ToListAsync();
        }

        private async Task<IEnumerable<FileMetaEntity?>> PrivateListAsync()
        => storage.File.AsNoTracking().OrderBy(e => e.FileName).AsEnumerable();

        public async Task<IEnumerable<FileMetaEntity?>> TryReadListAsync() => IsValid ? await PrivateListAsync() : await PublicListAsync();

        /// <summary>
        /// 预览&下载 API 服务方法 : 生成文件流,提供给 Controller 层直接返回给客户端, 解耦 IO 逻辑和 Web 层的细节
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="OperateTransactionFailedException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<(Stream stream, string contentType, string fileName)> GetFileInternalAsync(string? title, Guid? id)
        {
            #region 初步检查: 是否返回全文件列表 

            var fileInfo = await TryReadAsync(id, title) is IEnumerable<FileMetaEntity> files && files.Any() ? files.First() : throw new OperateTransactionFailedException("不存在文件记录");
            // 此处若得到授权则返回全文见列表,包含图像资源;后续步骤检查资源扩展名是否有效,若有效则允许下载?
            #endregion

            #region 检查下载权限：仅允许 AllowedDownload 中的扩展名
            //fileInfo.Select(e=> downloadPermissionCheck.Check(Path.GetExtension(e.DesensitizationName)) is (false,string msg) )
            //var fileExt = Path.GetExtension(fileInfo.FileName).ToLowerInvariant();

            //if (fileExt is null || !downloadPermissionCheck.Check(fileExt).IsValid)
            //{
            //    throw new OperateTransactionFailedException($"文件类型 {fileExt} 不允许下载");
            //}
            #endregion

            #region 允许下载后检查文件是否存在
            var fullPath = Path.Combine(policy.Value.StoragePath, fileInfo!.DesensitizationName+fileInfo.Extension);
            if (!File.Exists(fullPath)) throw new FileNotFoundException("源文件丢失,请联系管理员", fullPath);
            #endregion

            #region 释放下载或预览资源
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var ext = GetFileMimeType(fileInfo.DesensitizationName+fileInfo.Extension);
            return (stream, ext, fileInfo.FileName);
            #endregion
        }

        private async Task ValidateFileAsync(IFormFile file)
        {
            foreach (var strategy in uploadStrategies)
                if (strategy.Check(file) is (false, string message))
                    throw new OperateTransactionFailedException(message);

            if (await effectivenessCheck.CheckAsync(file) is (false, string msg))
                throw new OperateTransactionFailedException(msg);
        }
        public async Task<IEnumerable<FileMetaEntity>> TryCreateAsync(params IFormFile[] files)
        {
            ArgumentNullException.ThrowIfNull(files);
            if (files is not { Length: > 0 })
                throw new ArgumentException("至少需要上传一个文件", nameof(files));

            var fileMetas = new List<FileMetaEntity>();
            var savedPaths = new List<string>();

            try{
                foreach (var file in files)
                {
                    await ValidateFileAsync(file);
                    var meta = new FileMetaEntity(file, upContext.FileHash ?? throw new OperateTransactionFailedException("读取hash失败"));
                    var targetPath = Path.Combine(policy.Value.StoragePath, meta.DesensitizationName + meta.Extension);
                    await SaveFile(file, targetPath);
                    fileMetas.Add(meta);
                    savedPaths.Add(targetPath);
                }
                await storage.File.AddRangeAsync(fileMetas);
                await storage.SaveChangesAsync();
                return fileMetas;
            }
            catch
            {
                foreach (var path in savedPaths)
                    if (File.Exists(path)) File.Delete(path);
                throw;
            }
        }

        public async Task<int> TryDeleteAsync(Guid? id)
        {
            if (id is null) return 0;
            var file = await storage.File.FindAsync(id) ?? throw new FileNotFoundException("文件不存在");
            var fullPath = Path.Combine(policy.Value.StoragePath, file.DesensitizationName + file.Extension);
            if (File.Exists(fullPath)) File.Delete(fullPath);
            storage.File.Remove(file);
            return await storage.SaveChangesAsync();
        }

        public async Task<int> RemoveUnownedFile()
        {
            // 从 指定目录获取所有文件名称,并对数据库记录进行比对, 删除数据库中没有记录的文件, 避免垃圾文件占用存储空间
            var groupFile = Directory.GetFiles(policy.Value.StoragePath);
            var ownedFiles = await storage.File.Select(f => f.DesensitizationName + f.Extension).ToListAsync();
            var unownedFiles = groupFile.Where(f => !ownedFiles.Contains(Path.GetFileName(f)));

            foreach (var file in unownedFiles)
                File.Delete(file);

            return unownedFiles.Count();
        }
        private string GetFileMimeType(string filename)
        {
            if (_extProvider.TryGetContentType(filename, out var contentType))
                return contentType;
            throw new OperateTransactionFailedException("您请求的数据存在异常,已被拦截,若有疑问,请联系管理员");
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