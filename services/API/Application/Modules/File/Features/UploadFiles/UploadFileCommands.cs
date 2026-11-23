using AqLife.Application.Abstractions.Commands;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.File;
/// <summary>上传文件并将其设置为草稿或定时发布</summary>
public record CreateFileCommand(IFormFile File, [Required(ErrorMessage = "必须提供分类参数")] Guid CategoryId, IEnumerable<Guid>? Tags = null, DateTimeOffset? ScheduledTime = null, bool IsTemplate = false)
    : ICreateCommand, IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles() => [File];
}
/// <summary>多文件上传</summary>
public record UploadFilesCommand(Guid categoryId, params IFormFile[] Files) : ICreateCommand<IEnumerable<Guid>>, IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles() => Files;
}
