using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AqLife.Domain.Command
{
    public record BlogUnpublishQuery(DateTimeOffset ScheduledTime) : IQuery<IEnumerable<FileDto>>;
    public record FileQuery(Guid? UID = null, string? Title = null,Guid? CategoryUID=null, int Page=1,int PageSize=10) : IPageQuery, IQuery<PageResult<FileDto>>;
    public record DownloadFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FileDownloadModel>;
    public record PreviewFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FilePreviewModel>;
    public record DeleteFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IDeleteCommand;
    public record UpdateFileCommand(Guid UID, IFormFile File) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    public record UpdateFileTagCommand(Guid UID, IEnumerable<Guid> tags) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }
    /// <summary>
    /// 上传文件,并将其设置为草稿,除非手动使用预定,否则永不发布
    /// </summary>
    /// <param name="File"></param>
    public record CreateFileCommand(params IFormFile[] File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => File;
    }
    /// <summary>
    /// 预定时间发布,针对已上传文件
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="ScheduledAt"></param>
    public record ScheduledFileCommand(Guid UID,DateTimeOffset? ScheduledAt) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }
    /// <summary>
    /// 取消发布,用于改变已发布和计划发布的博文
    /// </summary>
    /// <param name="UID"></param>
    public record CancelScheduledFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
    /// <summary>
    /// 备选! 用于上传文件并设置延时发布,如果ScheduledTime为null,则默认为草稿
    /// </summary>
    /// <param name="File"></param>
    /// <param name="ScheduledTime"></param>
    public record PublishFileCommand(IFormFile File, DateTimeOffset? ScheduledTime=null) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
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
