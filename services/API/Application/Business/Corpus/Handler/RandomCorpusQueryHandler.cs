using AqLife.Application.Business.Corpus.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Business.Corpus.Handler
{
    public class RandomCorpusQueryHandler(CorpusSearch search) : IRequestHandler<RandomCorpusQuery, string>
    {
        public async Task<string> Handle(RandomCorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(new CorpusQuery(), ct);
            if (result.Count() == 0) throw new Exception("没有语料");
            var randomIndex = Random.Shared.Next(result.Count());
            CorpusEntity corpus = result.OrderBy(e => e.CorpusContent).Skip(randomIndex).First();
            //return mapper.ToDto(corpus);
            return corpus.CorpusContent;
        }
    }
}
