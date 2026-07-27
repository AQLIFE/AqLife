using MediatR;
using MyLife.Application.Business.Corpus.Search;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Corpus.Handler
{
    public class CorpusQueryHandler(CorpusSearch search, CorpusMapper mapper) : IRequestHandler<CorpusQuery, IEnumerable<CorpusDto>?>
    {
        public async Task<IEnumerable<CorpusDto>?> Handle(CorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(query, ct);
            return result?.Select(mapper.ToDto) ?? [];
        }
    }
}
