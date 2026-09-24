using AqLife.Application.Business.Corpus.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.Corpus;
using AqLife.Application.Mappers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Corpus.Handler
{
    public class CorpusQueryHandler(CorpusSearch search, PageResultMapper<CorpusEntity,CorpusDto> mapper) : IRequestHandler<CorpusQuery, PageResult<CorpusDto>>
    {
        public async Task<PageResult<CorpusDto>> Handle(CorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);
            return mapper.ToDto(result);
        }
    }
}
