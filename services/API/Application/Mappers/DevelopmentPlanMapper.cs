using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public sealed partial class DevelopmentPlanMapper :
        IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto>,
        ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand>
    {
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.Todos))]
        public partial DevelopmentPlanDto ToDto(DevelopmentPlanEntity entity);

        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.UID))]
        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.UpdatedAt))]
        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.IsArchived))]
        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.Todos))]
        [MapperIgnoreTarget(nameof(DevelopmentPlanEntity.DependencyVersion))]
        public partial DevelopmentPlanEntity ToEntity(CreateDevelopmentPlanCommand command);
    }
}