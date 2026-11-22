using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.Favorite;
using Core.Search;
using Favorite.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Favorite
{
    public static class Favorite
    {
        public static IServiceCollection AddFavorite(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>, AllFavoriteSearchStrategy>();
            services.AddScoped<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>, FilteredFavoriteSearchStrategy>();
            services.AddScoped<ISearch<FavoriteQuery, PageResult<FavoriteDto>, FavoriteItemEntity>, FavoriteSearch>();

            services.AddSingleton<FavoriteMediaMapper>();
            services.AddSingleton<FavoriteMapper>();
            services.AddSingleton<IViewMapper<FavoriteItemEntity, FavoriteDto>>(sp => sp.GetRequiredService<FavoriteMapper>());
            services.AddSingleton<ICreateMapper<FavoriteItemEntity, CreateFavoriteCommand>>(sp => sp.GetRequiredService<FavoriteMapper>());
            services.AddSingleton<ISearchMapper<FavoriteQuery, PageResult<FavoriteDto>, FavoriteSearchCriteria>>(sp => sp.GetRequiredService<FavoriteMapper>());

            services.AddScoped<IFileReferenceProvider, FavoriteFileReferenceProvider>();

            return services;
        }
    }
}
