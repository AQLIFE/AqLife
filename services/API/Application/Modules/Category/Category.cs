global using AqLife.Shared.IView;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Category.Search;
using AqLife.Domain.Entities;
using Category;
using Core.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Application.Category
{
    public static class Category
    {
        public static IServiceCollection AddCategory(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<CategoryEntity, EntitySearchCriteria>, AllCategorySearchStrategy>();
            services.AddScoped<ISearchStrategy<CategoryEntity, EntitySearchCriteria>, FilteredCategorySearchStrategy>();
            services.AddScoped<ISearch<CategoryQuery, PageResult<CategoryDto>, CategoryEntity>, CategorySearch>();

            services.AddSingleton<CategoryMapper>();
            services.AddSingleton<IViewMapper<CategoryEntity, CategoryDto>>(sp => sp.GetRequiredService<CategoryMapper>());
            services.AddSingleton<ICreateMapper<CategoryEntity, CreateCategoryCommand>>(sp => sp.GetRequiredService<CategoryMapper>());
            services.AddSingleton<ISearchMapper<CategoryQuery, PageResult<CategoryDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<CategoryMapper>());

            return services;
        }
    }
}