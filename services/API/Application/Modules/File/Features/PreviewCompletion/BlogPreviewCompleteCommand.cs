using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.File;
namespace AqLife.Application.File;
public record BlogPreviewCompleteCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
