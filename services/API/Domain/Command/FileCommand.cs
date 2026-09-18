using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AqLife.Domain.Command
{
    public record BlogUnpublishQuery(DateTimeOffset ScheduledTime) : IQuery<IEnumerable<FileDto>>;
    public record FileQuery(Guid? UID = null, string? Title = null) : IQuery<IEnumerable<FileDto>>;
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

    public record PublishFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }// 即时发布
    public record ScheduledBlogCommand(Guid UID,DateTimeOffset ScheduledAt) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }// 即时发布
    public record CancelScheduledBlogPostCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;//取消发布,用于改变已发布和计划发布的博文
    public record PublishScheduledCommand(DateTime? ScheduledTime, IFormFile File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    public record ProcessScheduledPostsCommand : IRequest<ProcessScheduledPostsResult>;// 需要手动注册相关依赖
    public class ProcessScheduledPostsResult(
    int Found,
    int Published,
    int Failed)
    {
        public int Found { get; set; } = Found;
        public int Published { get; set; } = Published;
        public int Failed { get; set; } = Failed;
    };
}
