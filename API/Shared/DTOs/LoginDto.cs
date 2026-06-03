namespace MyLife.Shared.DTOs
{
    public record LoginDto
    (
        string Name,
        string Password
    ):IEntityDto;
}
