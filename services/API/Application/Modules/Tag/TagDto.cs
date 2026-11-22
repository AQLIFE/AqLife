using AqLife.Shared.IView;

namespace Tag
{
    public record TagDto(string Uid, string Name, string? AliasName = null) : IEntityDto;
}
