using AqLife.Shared.IView;
using AqLife.Application.Abstractions.Search;
using AqLife.Shared.Options;
using System.Text.Json.Serialization;

namespace File
{
    // File owns these projection contracts so its public response shape does not depend on other modules.
    public record FileCategorySummaryDto(
        Guid Uid,
        string CategoryName,
        string CategoryDescription,
        int Count = 0,
        Guid? ParentId = null,
        IEnumerable<Guid>? ResultIds = null);

    public record FileTagSummaryDto(string Uid, string Name, string? AliasName = null);

    public record FileDto
    (
        Guid UID,
        string FileName,
        FileCategorySummaryDto Category,
        IEnumerable<FileTagSummaryDto>? Tags,
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