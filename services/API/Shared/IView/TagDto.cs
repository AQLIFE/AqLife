namespace AqLife.Shared.IView
{
    public record TagDto(string Uid, string Name, string? AliasName = null) : IEntityDto;

    public record CategoryDto(Guid Uid, string CategoryName, string CategoryDescription, int FileCount,int FavoriteCount, string? ParentName) : IEntityDto;

}
