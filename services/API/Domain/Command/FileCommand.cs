using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;

namespace AqLife.Domain.Command
{
    public record FileQuery(Guid? UID = null, string? Title = null) : IQuery<IEnumerable<FileDto>>;
    public record DownloadFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FileDownloadModel>;
    public record PreviewFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FilePreviewModel>;
    public record DeleteFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IDeleteCommand;
    public record UpdateFileCommand(Guid UID, IFormFile File) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    public record UpdateFileTagCommand(Guid UID, IEnumerable<Guid> tags) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }
    public record PublishFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }

    public record CreateFileCommand(params IFormFile[] File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => File;
    }

    public record PublishScheduledCommand(DateTime? ScheduledTime, IFormFile File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
}
