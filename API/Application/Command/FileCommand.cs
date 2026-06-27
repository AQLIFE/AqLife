using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Shared.Command;
using MyLife.Shared.DTOs;

namespace MyLife.Application.Command
{
    public record GetFileMetadataQuery(Guid? UID = null, string? Title = null) : IQuery<IEnumerable<FileMetadataDto>>;
    public record DownloadFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FileDownloadModel>;
    public record PreviewFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FilePreviewModel>;
    public record DeleteFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IDeleteCommand;
    public record UpdateFileCommand(Guid UID, IFormFile File) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    public record UpdateFileTagCommand(Guid UID, IEnumerable<Guid> tags) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }

    public record CreateFileCommand(params IFormFile[] File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => File;
    }
}
