using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Handler;

public sealed class CreateCategoryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        CategoryEntity? parent = command.ParentId is Guid parentId
            ? await dbContext.Categories.SingleOrDefaultAsync(category => category.UID == parentId, ct)
            : null;

        var entity = new CategoryEntity(
            command.Name.Trim(),
            command.desc?.Trim() ?? string.Empty,
            parent);

        await dbContext.Categories.AddAsync(entity, ct);
        return entity.UID;
    }
}