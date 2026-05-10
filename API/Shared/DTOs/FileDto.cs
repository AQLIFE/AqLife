namespace MyLife.Shared.DTOs
{
    public record FileDto
    (
        string FileName,
        ulong FileSize = 0u,
        string FileHash = "",
        string UploadTime = ""
    );
}
