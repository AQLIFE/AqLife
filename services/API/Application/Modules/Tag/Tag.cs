global using AqLife.Shared.IView;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Core.Search;
using Microsoft.Extensions.DependencyInjection;
using Tag.Search;

namespace Tag
{
    public static class Tag
    {
        public static IServiceCollection AddTag(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, AllTagSearchStrategy>();
            services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, FilterTagSearchStrategy>();
            services.AddScoped<ISearch<TagQuery, PageResult<TagDto>, TagEntity>, TagSearch>();

            services.AddSingleton<TagMapper>();
            services.AddSingleton<IViewMapper<TagEntity, TagDto>>(sp => sp.GetRequiredService<TagMapper>());
            services.AddSingleton<ICreateMapper<TagEntity, CreateTagCommand>>(sp => sp.GetRequiredService<TagMapper>());
            services.AddSingleton<ISearchMapper<TagQuery, PageResult<TagDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<TagMapper>());

            return services;
        }
    }
}