using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Category;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Category.Handler;

public sealed class UpdateCategoryHandler(ICategoryPersistence persistence)
    : IRequestHandler<UpdateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var entity = await persistence.Categories
            .SingleOrDefaultAsync(category => category.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("Category 不存在");

        CategoryEntity? parent = command.ParentId is Guid parentId
            ? await persistence.Categories.SingleOrDefaultAsync(category => category.UID == parentId, ct)
            : null;

        entity.Update(command.Name.Trim(), command.Desc?.Trim() ?? string.Empty);
        entity.SetParentCategory(parent);
        return entity.UID;
    }
}