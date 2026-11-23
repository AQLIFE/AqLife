using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.File;
using Microsoft.AspNetCore.Http;
namespace AqLife.Application.File;
public record UpdateFileCommand(Guid UID, IFormFile File) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand, IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles() => [File];
}
public record UpdateFileTagCommand(Guid UID, IEnumerable<Guid> tags) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
public record UpdateFileCategoryCommand(Guid UID, Guid categoryId) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
