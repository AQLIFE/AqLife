using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Exceptions;
using Category;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Category.Handler;

public sealed class DeleteCategoryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var entity = await dbContext.Categories
            .SingleOrDefaultAsync(category => category.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("Category 不存在");

        dbContext.Categories.Remove(entity);
        return Unit.Value;
    }
}