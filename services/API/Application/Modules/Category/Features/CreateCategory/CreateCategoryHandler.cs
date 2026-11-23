using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Category.Handler;

public sealed class CreateCategoryHandler(ICategoryPersistence persistence, ICreateMapper<CategoryEntity, CreateCategoryCommand> createMapper)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var entity = createMapper.ToEntity(command);
        await persistence.AddCategoryAsync(entity, ct);
        return entity.UID;
    }
}