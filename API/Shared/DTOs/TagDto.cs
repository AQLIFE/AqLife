namespace MyLife.Shared.DTOs
{
    public record TagDto(string Name, string? AliasName = null, bool IsCategory = false);
}
