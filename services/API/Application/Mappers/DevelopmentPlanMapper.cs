using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public sealed partial class DevelopmentPlanMapper :
        IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto>,
        ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand>,
        ISearchMapper<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, EntitySearchCriteria>,
        ISearchMapper<TodoRoadmapQuery, TodoRoadmapDto, EntitySearchCriteria>
    {
        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        [MapperIgnoreSource(nameof(TodoRoadmapQuery.PlanId))]
        public partial EntitySearchCriteria ToCriteria(TodoRoadmapQuery query);

        [MapProperty(nameof(DevelopmentPlanQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(DevelopmentPlanQuery query);

        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.Todos))]
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.DependencyVersion))]
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