namespace MyLife.Shared.DTOs
{
    public record FileMetadataDto
    (
        Guid UID,
        string FileName,
        IEnumerable<TagDto>? Tags,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = "",
        string FileType = ""
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
