namespace MyLife.Shared.IView
{
    public record TagDto(string Uid, string Name, string? AliasName = null, bool IsCategory = false) : IEntityDto;
}
