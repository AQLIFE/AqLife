using MediatR;
using MyLife.Application.Business.Corpus.Search;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Corpus.Handler
{
    public class RandomCorpusQueryHandler(CorpusSearch search, CorpusMapper mapper) : IRequestHandler<RandomCorpusQuery, CorpusDto>
    {
        public async Task<CorpusDto> Handle(RandomCorpusQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(new CorpusQuery(), ct);
            if (result.Count() == 0) throw new Exception("没有语料");
            var randomIndex = new Random().Next(result.Count());
            CorpusEntity corpus = result.OrderBy(e => e.CorpusContent).Skip(randomIndex).Single();
            return mapper.ToDto(corpus);
        }
    }
}
