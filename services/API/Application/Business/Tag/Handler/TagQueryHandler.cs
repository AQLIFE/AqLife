using AqLife.Application.Business.Tag.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.Tag;

namespace AqLife.Application.Business.Tag.Handler
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
