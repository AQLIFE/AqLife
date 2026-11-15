using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Handler;

public sealed class UpdateCategoryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var entity = await dbContext.Categories
            .SingleOrDefaultAsync(category => category.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("Category 不存在");

        CategoryEntity? parent = command.ParentId is Guid parentId
            ? await dbContext.Categories.SingleOrDefaultAsync(category => category.UID == parentId, ct)
            : null;

        entity.Update(command.Name.Trim(), command.Desc?.Trim() ?? string.Empty);
        entity.SetParentCategory(parent);
        return entity.UID;
    }
}