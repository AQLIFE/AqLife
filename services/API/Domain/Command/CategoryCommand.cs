using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command
{
    public enum CategoryQueryType
    {
        Blog,
        Favorite
    }

    /// <summary>
    /// 查询分类及其关联业务数量。CID 指定时仅返回对应分类。
    /// </summary>
    public record CategoryQuery(Guid? CID = null, CategoryQueryType SearchType = CategoryQueryType.Blog)
        : IQuery<PageResult<CategoryDto>?>;

    public record CreateCategoryCommand(
        [property: Required, StringLength(32)] string Name,
        [property: StringLength(256)] string desc,
        Guid? ParentId) : ICreateCommand;

    public record UpdateCategoryCommand(
        Guid UID,
        [property: Required, StringLength(32)] string Name,
        [property: StringLength(256)] string desc,
        Guid? ParentId) : IUpdateCommand, IRequireValidEntity<CategoryEntity>;

    public record DeleteCategoryCommand(Guid UID)
        : IDeleteCommand, IRequireValidEntity<CategoryEntity>;
}