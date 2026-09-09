namespace AqLife.Shared.IView
{
    public record FileDto
    (
        Guid UID,
        string FileName,
        IEnumerable<TagDto>? Tags,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = "",
        string FileType = "",
        string FileIntroduction = ""
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
