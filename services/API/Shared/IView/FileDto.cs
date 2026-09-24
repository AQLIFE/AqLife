using AqLife.Shared.Options;
using System.Text.Json.Serialization;

namespace AqLife.Shared.IView
{
    public record FileDto
    (
        Guid UID,
        string FileName,
        IEnumerable<TagDto>? Tags,
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        FileStatus? PublishStatus,
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        DateTimeOffset? PublishAt,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = "",
        string FileType = "",
        string FileIntroduction = "",
        int ViewCount = 0
    ) : IEntityDto;

    public record FileDownloadModel(
    Stream FileStream,
    string ContentType,
    string FileName
    ) : FilePreviewModel(FileStream, ContentType);

    public record FilePreviewModel(
    Stream FileStream,
    string ContentType
    );
}
