namespace MyLife.Shared.DTOs
{
    public record FileDto
    (
        Guid UID,
        string FileName,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = ""
    ):IEntityDto;
}
