using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Category;

public record DeleteCategoryCommand(Guid UID)
    : IDeleteCommand, IRequireValidEntity<CategoryEntity>;
