using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Handler;

public sealed class CreateCategoryHandler(IApplicationDbContext dbContext,ICreateMapper<CategoryEntity,CreateCategoryCommand> createMapper)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var entity = createMapper.ToEntity(command);

        await dbContext.Categories.AddAsync(entity, ct);
        return entity.UID;
    }
}