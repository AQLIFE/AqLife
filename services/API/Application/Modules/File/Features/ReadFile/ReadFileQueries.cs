using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.File;
namespace AqLife.Application.File;
public record DownloadFileQuery(Guid UID) : IRequireValidEntity<FileMetaEntity>, IQuery<FileDownloadModel>;
public record PreviewFileQuery(Guid UID, bool IsTemplate = false) : IRequireValidEntity<FileMetaEntity>, IQuery<FilePreviewModel>;
