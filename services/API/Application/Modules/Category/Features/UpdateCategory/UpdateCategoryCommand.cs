using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Application.Category;

public record UpdateCategoryCommand(
    Guid UID,
    [Required, StringLength(32)] string Name,
    [StringLength(256)] string Desc,
    Guid? ParentId) : IUpdateCommand, IRequireValidEntity<CategoryEntity>;
