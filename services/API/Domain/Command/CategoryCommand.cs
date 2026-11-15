using AqLife.Domain.Entities;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command
{
    public enum CategoryQueryType
    {
        Default,
        File,
        Favorite
    }

    /// <summary>
    /// 查询分类及其关联业务数量。CID 指定时仅返回对应分类。
    /// </summary>
    public record CategoryQuery(Guid? UID = null, CategoryQueryType SearchType = CategoryQueryType.File, int Page = 1, int PageSize = 10)
        : IQuery<PageResult<CategoryDto>>, IPageQuery;

    public record CreateCategoryCommand(
        [Required, StringLength(32,ErrorMessage = "分类名称不允许超过32")] string Name,
        [StringLength(256,ErrorMessage = "分类描述不允许超过256")] string Desc,
        Guid? ParentId=null) : ICreateCommand;

    public record UpdateCategoryCommand(
        Guid UID,
        [Required, StringLength(32)] string Name,
        [StringLength(256)] string Desc,
        Guid? ParentId) : IUpdateCommand, IRequireValidEntity<CategoryEntity>;

    public record DeleteCategoryCommand(Guid UID)
        : IDeleteCommand, IRequireValidEntity<CategoryEntity>;
}