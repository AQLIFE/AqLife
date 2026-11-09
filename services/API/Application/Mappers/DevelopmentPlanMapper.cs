using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;

namespace AqLife.Application.Mappers
{
    public sealed class DevelopmentPlanMapper :
        IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto>,
        ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand>
    {
        public DevelopmentPlanDto ToDto(DevelopmentPlanEntity entity)
            => new(
                entity.UID,
                entity.Name,
                entity.Description,
                entity.IsArchived,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.DependencyVersion);

        public DevelopmentPlanEntity ToEntity(CreateDevelopmentPlanCommand command)
            => new(command.Name, command.Description);
    }
}