using AqLife.Shared.Options;
using System.Text.Json.Serialization;

namespace AqLife.Shared.IView
{
    public record FileDto
    (
        Guid UID,
        string FileName,
        CategoryDto Category,
        IEnumerable<TagDto>? Tags,
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        FileStatus? PublishStatus,
        [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        DateTimeOffset? PublishAt,// 是否应该提供精确日期和时间? 矛盾在于 web 项目不应该得到详细时间 ,但 mgs项目需要,所以应该在Search 上解决; 嗯 应该如此
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
    ):PageResult<FileDto>(RecentFiles,Page,PageSize,HasMore);
}
