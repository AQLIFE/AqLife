namespace MyLife.Shared.DTOs
{
    public record FileMetadataDto
    (
        Guid UID,
        string FileName,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = ""
    ):IEntityDto;

    public record FileDownloadModel(
    Stream FileStream,
    string ContentType,
    string FileName
    ): FilePreviewModel(FileStream,ContentType);

    public record FilePreviewModel(
    Stream FileStream,
    string ContentType
    );
}
