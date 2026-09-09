using AqLife.Application.Business.Corpus.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.Corpus;

namespace AqLife.Application.Business.Corpus.Handler
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
