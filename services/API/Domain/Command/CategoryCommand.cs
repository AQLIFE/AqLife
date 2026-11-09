using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;

namespace AqLife.Domain.Command
{
    public enum CategoryQueryType
    {
        Blog,
        Favorite
    }

    /// <summary>
    /// 分类查询
    /// </summary>
    /// <param name="CID">当存在该参数时,会根据对应 SeafchType 参数进行搜索</param>
    /// <param name="SearchType"></param>
    public record CategoryQuery(Guid? CID = null, CategoryQueryType SearchType = CategoryQueryType.Blog) : IQuery<PageResult<CategoryDto>?>;

    public record CreateCategoryCommand(string Name, string desc, Guid? ParentId) : ICreateCommand;
    public record UpdateCategoryCommand(Guid UID, string Name, string desc, Guid? ParentId) : IUpdateCommand, IRequireValidEntity<CategoryEntity>;
    public record DeleteCategoryCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<CategoryEntity>;
}
