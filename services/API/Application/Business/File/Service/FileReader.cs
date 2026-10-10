using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Business.File.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.StaticFiles;

namespace AqLife.Application.Business.File.Service
{
    public class FileReader(FileExtensionContentTypeProvider extProvider, FileSearch fileSearch, IFileStorage fileStorage)
    {
        private string GetFileMimeType(string filename)
        {
            if (extProvider.TryGetContentType(filename, out var contentType))
                return contentType;
            throw new FileValidationException("您请求的数据存在异常,已被拦截,若有疑问,请联系管理员");
        }
        /// <summary>
        /// 读取文件流,但可根据isTemplate参数来判断是否是模板文件,若是模板文件,则只允许读取模板文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isTemplate"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="ResourceNotFoundException"></exception>
        public async Task<FileDownloadModel> ReadAsync(Guid id,FileScope scope=FileScope.All, CancellationToken ct=default)
        {
            FileMetaEntity fileInfo = (await fileSearch.SearchAsync(query: new FileQuery(UID: id,Scope:scope), ct)).FirstOrDefault() ?? throw new ResourceNotFoundException("不存在文件记录");
            Stream stream = await fileStorage.OpenReadAsync(fileInfo.StorageKey, ct);
            var contentType = GetFileMimeType(fileInfo.StorageKey);
            return new FileDownloadModel(stream, contentType, fileInfo.FileName);
        }

        public async Task<string> GetContentAsync(string fileName,CancellationToken ct)
        {
            await using var stream = await fileStorage.OpenReadAsync(fileName, ct);

            using var reader = new StreamReader(stream);

            return  await reader.ReadToEndAsync(ct);
        }
    }
}
