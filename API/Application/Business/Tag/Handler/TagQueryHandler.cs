using MediatR;
using MyLife.Application.Business.Tag.Search;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Tag.Handler
{
    public class TagQueryHandler(TagSearch search, TagMapper tagMapper) : IRequestHandler<TagQuery, IEnumerable<TagDto>?>
    {
        public async Task<IEnumerable<TagDto>?> Handle(TagQuery query, CancellationToken ct)
        {
            var entites = await search.SearchAsync(query, ct);
            return entites?.Select(e => tagMapper.ToDto(e!)) ?? [];
        }
    }
}
