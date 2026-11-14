namespace AqLife.Shared.IView
{
    public record TagDto(string Uid, string Name, string? AliasName = null) : IEntityDto;

    public record CategoryDto(Guid Uid, string CategoryName, string CategoryDescription, int Count=0, Guid? ParentId=null,IEnumerable<Guid>? ResultIds = null) : IEntityDto;



}
