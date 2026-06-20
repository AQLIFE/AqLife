using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Shared;
using MyLife.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Command
{
    public record GetFileMetadataQuery(Guid? UID = null, string? Title = null) : IQuery<IEnumerable<FileMetadataDto>>;
    public record DownloadFileQuery(Guid UID) : IMustCheckExistence<FileMetaEntity>,IQuery<FileDownloadModel>;
    public record PreviewFileQuery(Guid UID) :  IMustCheckExistence<FileMetaEntity>,IQuery<FilePreviewModel>;
    public record DeleteFileCommand(Guid UID):  IMustCheckExistence<FileMetaEntity>, IDeleteCommand;
    public record UpdateFileCommand(Guid UID,IFormFile File) : IMustCheckExistence<FileMetaEntity>, IUpdateCommand<Guid>, IUploadRequest
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    public record CreateFileCommand(params IFormFile[] File) : ICreateCommand<IEnumerable<Guid>>, IUploadRequest
    {
        public IEnumerable<IFormFile> GetFiles() => File;
    }
}
