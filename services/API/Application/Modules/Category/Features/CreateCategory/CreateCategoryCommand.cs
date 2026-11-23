using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Application.Category;

public record CreateCategoryCommand(
    [Required, StringLength(32, ErrorMessage = "分类名称不允许超过32")] string Name,
    [StringLength(256, ErrorMessage = "分类描述不允许超过256")] string Desc,
    Guid? ParentId = null) : ICreateCommand;
