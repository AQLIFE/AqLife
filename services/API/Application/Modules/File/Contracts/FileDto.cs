using AqLife.Shared.IView;
using AqLife.Application.Abstractions.Search;
using AqLife.Shared.Options;
using System.Text.Json.Serialization;

namespace AqLife.Application.File
{
    public record FileDto
    (
        Guid UID,
        string FileName,
        CategoryDto Category,
        IEnumerable<TagDto>? Tags,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        FileStatus? PublishStatus,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        DateTimeOffset? PublishAt,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = "",
        string FileType = "",
        string FileIntroduction = "",
        int ViewCount = 0,
        int Version = 1,
        bool IsTemplate = false
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

    public record FileOverviewDto(
        int DraftCount,
        int ScheduledCount,
        int PublishedCount,
        List<FileDto> RecentFiles,
        int Page,
        int PageSize,
        PublishStatusSearch SearchStatus = PublishStatusSearch.Default,
        bool HasMore = false
    ) : PageResult<FileDto>(RecentFiles, Page, PageSize, HasMore);
}