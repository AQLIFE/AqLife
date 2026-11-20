using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Modules.Category.Handler;

public sealed class CreateCategoryHandler(IApplicationDbContext dbContext, ICreateMapper<CategoryEntity, CreateCategoryCommand> createMapper)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var entity = createMapper.ToEntity(command);
        await dbContext.Categories.AddAsync(entity, ct);
        return entity.UID;
    }
}