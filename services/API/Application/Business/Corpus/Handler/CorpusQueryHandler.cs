using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.Corpus.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;

namespace AqLife.Application.Business.Corpus.Handler
{
    public class CorpusQueryHandler(ISearch<CorpusQuery, CorpusEntity> search, PageResultMapper<CorpusEntity, CorpusDto> mapper) : IRequestHandler<CorpusQuery, PageResult<CorpusDto>>
    {
        public async Task<PageResult<CorpusDto>> Handle(CorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);
            return mapper.ToDto(result);
        }
    }
}
