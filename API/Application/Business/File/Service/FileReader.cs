using Microsoft.AspNetCore.StaticFiles;
using MyLife.Application.Abstractions.FileStorage;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.File.Service
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
            string storageFileName = fileInfo.UID + fileInfo.Extension;
            if (!fileStorage.Exists(storageFileName)) throw new ResourceNotFoundException("源文件丢失,请联系管理员:" + fileInfo.FileName);

            Stream stream = fileStorage.OpenRead(storageFileName);
            var contentType = GetFileMimeType(storageFileName);
            return new FileDownloadModel(stream, contentType, fileInfo.FileName);
        }
    }
}
