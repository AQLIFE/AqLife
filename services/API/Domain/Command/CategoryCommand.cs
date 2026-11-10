using AqLife.Domain.Entities;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command
{
    public enum CategoryQueryType
    {
        File,
        Favorite
    }

    /// <summary>
    /// 查询分类及其关联业务数量。CID 指定时仅返回对应分类。
    /// </summary>
    public record CategoryQuery(Guid? UID = null, CategoryQueryType SearchType = CategoryQueryType.File,int Page =1,int PageSize = 10)
        : IQuery<PageResult<CategoryDto>?>,IPageQuery;

    public record CreateCategoryCommand(
        [property: Required, StringLength(32)] string Name,
        [property: StringLength(256)] string Desc,
        Guid? ParentId) : ICreateCommand;

    public record UpdateCategoryCommand(
        Guid UID,
        [property: Required, StringLength(32)] string Name,
        [property: StringLength(256)] string Desc,
        Guid? ParentId) : IUpdateCommand, IRequireValidEntity<CategoryEntity>;

    public record DeleteCategoryCommand(Guid UID)
        : IDeleteCommand, IRequireValidEntity<CategoryEntity>;
}