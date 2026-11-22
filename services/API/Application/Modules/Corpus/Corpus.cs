using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.Corpus;
using AqLife.Domain.Entities;
using Core.Search;
using Corpus.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Corpus
{
    public static class Corpus
    {
        public static IServiceCollection AddCorpus(this IServiceCollection services)
        {
            services.AddScoped<ISearchStrategy<CorpusEntity, EntitySearchCriteria>, AllCorpusSearchStrategy>();
            services.AddScoped<ISearchStrategy<CorpusEntity, EntitySearchCriteria>, FilteredCorpusSearchStrategy>();
            services.AddScoped<ISearch<CorpusQuery, PageResult<CorpusDto>, CorpusEntity>, CorpusSearch>();

            services.AddSingleton<CorpusMapper>();
            services.AddSingleton<IViewMapper<CorpusEntity, CorpusDto>>(sp => sp.GetRequiredService<CorpusMapper>());
            services.AddSingleton<ICreateMapper<CorpusEntity, CreateCorpusCommand>>(sp => sp.GetRequiredService<CorpusMapper>());
            services.AddSingleton<ISearchMapper<CorpusQuery, PageResult<CorpusDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<CorpusMapper>());

            return services;
        }
    }
}
