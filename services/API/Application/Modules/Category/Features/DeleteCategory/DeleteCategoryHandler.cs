using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Category;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Category.Handler;

public sealed class DeleteCategoryHandler(ICategoryPersistence persistence)
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var entity = await persistence.Categories
            .SingleOrDefaultAsync(category => category.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("Category 不存在");

        persistence.RemoveCategory(entity);
        return Unit.Value;
    }
}