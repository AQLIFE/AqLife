using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Core.Mappers;
using MediatR;
using Tag;

namespace Tag.Handler
{
    public class TagQueryHandler(ISearch<TagQuery, PageResult<TagDto>, TagEntity> search, PageResultMapper<TagEntity, TagDto> mapper) : IRequestHandler<TagQuery, PageResult<TagDto>?>
    {
        public async Task<PageResult<TagDto>?> Handle(TagQuery query, CancellationToken ct)
        {
            var entites = await search.SearchPageAsync(query, ct);
            return mapper.ToDto(entites);
        }
    }
}
