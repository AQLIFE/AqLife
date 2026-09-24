namespace AqLife.Shared.IView
{
    public record TagDto(string Uid, string Name, string? AliasName = null, bool IsCategory = false) : IEntityDto;

    public record BlogCategoryStatistics(string Uid,string CategoryName,int CategoryCount) : IEntityDto;
    
}
