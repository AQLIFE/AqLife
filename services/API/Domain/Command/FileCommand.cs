using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AqLife.Domain.Command
{
    /// <summary>
    /// 获取处于预约状态但尚未发布的博客文章
    /// </summary>
    /// <param name="ScheduledTime"></param>
    public record BlogUnpublishQuery(DateTimeOffset ScheduledTime) : IQuery<IEnumerable<FileDto>>;
    /// <summary>
    /// 阅览完成,执行对应事务
    /// </summary>
    /// <param name="UID"></param>
    public record BlogPreviewCompleteCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
    /// <summary>
    /// 通用的文件查询,可用于获取文件列表,支持分页和排序
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Title"></param>
    /// <param name="CategoryUID"></param>
    /// <param name="Page"></param>
    /// <param name="PageSize"></param>
    /// <param name="Order"></param>
    /// <param name="Scope">查询文件的类型</param>
    public record FileQuery(Guid? UID = null, string? Title = null,Guid? CategoryUID=null, int Page=1,int PageSize=10, FileOrder Order= FileOrder.Latest, FileScope Scope = FileScope.Blog) : IPageQuery, IQuery<PageResult<FileDto>>;
    /// <summary>
    /// 下载文件,返回文件流和相关元数据
    /// </summary>
    /// <param name="UID"></param>
    public record DownloadFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FileDownloadModel>;
    /// <summary>
    /// 预览文件,返回文件流和相关元数据
    /// </summary>
    /// <param name="UID"></param>
    public record PreviewFileQuery(Guid UID,bool IsTemplate=false) : IRequireValidEntity<FileMetaEntity>, IQuery<FilePreviewModel>;
    /// <summary>
    /// 删除文件,同时删除相关的元数据和文件存储
    /// </summary>
    /// <param name="UID"></param>
    public record DeleteFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IDeleteCommand;
    /// <summary>
    /// 更新文件,同时更新相关的元数据和文件存储
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="File"></param>
    public record UpdateFileCommand(Guid UID, IFormFile File) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    /// <summary>
    /// 更新文件标签,用于批量更新文件的标签信息
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="tags"></param>
    public record UpdateFileTagCommand(Guid UID, IEnumerable<Guid> tags) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand { }
    /// <summary>
    /// 上传文件,并将其设置为草稿,除非手动使用预定,否则永不发布
    /// </summary>
    /// <param name="File"></param>
    public record CreateFileCommand(bool IsTemplate = false, params IFormFile[] File) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => File;
    }
    /// <summary>
    /// 预定时间发布,针对已上传文件,也可用于立即上传,或修改预定时间
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
    [Obsolete("Use ScheduledFileCommand instead")]
    public record PublishFileCommand(IFormFile File, DateTimeOffset? ScheduledTime=null) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles() => [File];
    }
    /// <summary>
    /// 处理所有已到期的预约发布,将其发布到指定位置,并返回处理结果:后台任务
    /// </summary>
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
