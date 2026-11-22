using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts;
using Core.Search;
using Microsoft.Extensions.DependencyInjection;
using Plan.DevelopmentPlan;
using Plan.DevelopmentPlan.Search;
using Plan.DevelopmentPlan.Validator;
using Plan.Todo;
using Plan.Todo.Search;
using Plan.Todo.Validator;

namespace Plan
{
    public static class Plan
    {
        public static IServiceCollection AddPlan(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<TodoEntity, TodoSearchCriteria>, AllTodoSearchStrategy>();
            services.AddScoped<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>, AllDevPlanSearchStrategy>();
            services.AddScoped<ISearchStrategy<TodoEntity, TodoSearchCriteria>, FilterTodoSearchStrategy>();
            services.AddScoped<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>, FilteredDevPlanSearchStrategy>();
            services.AddScoped<ISearch<TodoQuery, PageResult<TodoDto>, TodoEntity>, TodoSearch>();
            services.AddScoped<ISearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, DevelopmentPlanEntity>, DevelopmentPlanSearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>>();
            services.AddScoped<ISearch<TodoRoadmapQuery, TodoRoadmapDto, DevelopmentPlanEntity>, DevelopmentPlanSearch<TodoRoadmapQuery, TodoRoadmapDto>>();

            services.AddSingleton<TodoMapper>();
            services.AddSingleton<IViewMapper<TodoEntity, TodoDto>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<IViewMapper<RoadmapNodeLayoutEntity, RoadmapNodeLayoutDto>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<IViewMapper<DevelopmentPlanEntity, TodoRoadmapDto>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<IViewMapper<TodoDependencyEntity, TodoDependencyDto>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<ICreateMapper<TodoEntity, CreateTodoCommand>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<IBaseEntityMapper<RoadmapNodeLayoutEntity, SaveRoadmapNodeLayoutCommand>>(sp => sp.GetRequiredService<TodoMapper>());
            services.AddSingleton<ISearchMapper<TodoQuery, PageResult<TodoDto>, TodoSearchCriteria>>(sp => sp.GetRequiredService<TodoMapper>());

            services.AddSingleton<DevelopmentPlanMapper>();
            services.AddSingleton<IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
            services.AddSingleton<ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
            services.AddSingleton<ISearchMapper<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
            services.AddSingleton<ISearchMapper<TodoRoadmapQuery, TodoRoadmapDto, EntitySearchCriteria>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());

            services.AddScoped(typeof(IValidator<>), typeof(DevPlanValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(TodoUpdateValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(TodoAttributionValidityValidator<>));

            return services;
        }
    }
}
