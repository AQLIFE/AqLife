using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.File;
namespace AqLife.Application.File;
public record DeleteFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IDeleteCommand;
