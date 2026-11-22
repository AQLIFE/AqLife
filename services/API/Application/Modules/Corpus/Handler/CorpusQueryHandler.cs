using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Core.Mappers;
using Corpus;
using MediatR;

namespace Corpus.Handler
{
    public class CorpusQueryHandler(ISearch<CorpusQuery, PageResult<CorpusDto>, CorpusEntity> search, PageResultMapper<CorpusEntity, CorpusDto> mapper) : IRequestHandler<CorpusQuery, PageResult<CorpusDto>>
    {
        public async Task<PageResult<CorpusDto>> Handle(CorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);
            return mapper.ToDto(result);
        }
    }
}
