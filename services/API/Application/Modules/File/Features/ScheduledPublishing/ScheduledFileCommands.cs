using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.File;
namespace AqLife.Application.File;
public record ScheduledFileCommand(Guid UID, DateTimeOffset? ScheduledAt) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
public record CancelScheduledFileCommand(Guid UID) : IRequireValidEntity<FileMetaEntity>, IUpdateCommand;
