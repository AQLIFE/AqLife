using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Business.File.Search;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
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

        public async Task<FileDownloadModel> ReadAsync(Guid id, CancellationToken ct)
        {
            FileMetaEntity fileInfo = (await fileSearch.SearchAsync(query: new FileQuery(UID: id), ct)).FirstOrDefault() ?? throw new ResourceNotFoundException("不存在文件记录");

            if (!fileStorage.Exists(fileInfo.StorageName)) throw new ResourceNotFoundException("源文件丢失,请联系管理员:" + fileInfo.FileName);

            Stream stream = fileStorage.OpenRead(fileInfo.StorageName);
            var contentType = GetFileMimeType(fileInfo.StorageName);
            return new FileDownloadModel(stream, contentType, fileInfo.FileName);
        }
    }
}
