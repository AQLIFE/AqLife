namespace AqLife.Shared.IView
{
    public interface IEntityDto { }

    public record CategoryDto(
        Guid Uid,
        string CategoryName,
        string CategoryDescription,
        int Count = 0,
        Guid? ParentId = null,
        IEnumerable<Guid>? ResultIds = null) : IEntityDto;

    public record TagDto(string Uid, string Name, string? AliasName = null) : IEntityDto;
}