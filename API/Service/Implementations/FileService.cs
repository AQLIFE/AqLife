using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Service.Strategies;
using MyLife.Shared.Accident;
using MyLife.Shared.Options;
using Microsoft.AspNetCore.StaticFiles;

namespace MyLife.Service.Implementations
{
    public class FileService(
        IUploadStrategyAsync effectivenessCheck,

        IOptions<FilePolicyOption> _policy,
        AppStorage _storage,
        UploadContext _upContext,
        FileExtensionContentTypeProvider _extProvider,//使用框架内置服务
        IFileSearch _fileProvider)
    {

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
            var fileInfo = await _fileProvider.FindFileAsync(title, id,true) is IEnumerable<FileMetaEntity> files ?files.FirstOrDefault() : throw new OperateTransactionFailedException("数据库无记录");
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
            if (fileInfo is null) throw new OperateTransactionFailedException("不存在文件记录");
            var fullPath = Path.Combine(_policy.Value.StoragePath, fileInfo.DesensitizationName);
            if (!File.Exists(fullPath))throw new FileNotFoundException("源文件丢失,请联系管理员", fullPath);
            #endregion

            #region 释放下载或预览资源
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var ext = GetFileMimeType(fileInfo.DesensitizationName);
            return (stream, ext, fileInfo.FileName);
            #endregion
        }

        public async Task<FileMetaEntity> HandleUploadAsync(IFormFile file)
        {
            #region 上传事务的前置检查                  
            //foreach (var strategy in uploadStrategies)
            //    if( strategy.Check(file) is (false, string message))
            //        throw new OperateTransactionFailedException(message);
            
            if (await effectivenessCheck.CheckAsync(file) is (false,string msg))
                throw new OperateTransactionFailedException(msg);
            #endregion

            #region 上传&保存文件
            var fileMeta = new FileMetaEntity(file, _upContext.FileHash ?? throw new OperateTransactionFailedException("读取hash失败"));

            var targetPath = Path.Combine(_policy.Value.StoragePath, fileMeta.DesensitizationName);
            await SaveFile(file, targetPath);


            _storage.File.Add(fileMeta);
            await _storage.SaveChangesAsync();
            return fileMeta;
            #endregion
        }


        private string GetFileMimeType(string filename)
        {
            var ext = Path.GetExtension(filename).ToLowerInvariant();
            if ( _extProvider.TryGetContentType(filename, out var contentType))
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