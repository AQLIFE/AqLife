using AqLife.Application.Business.Tag;
using AqLife.Application.Business.Tag.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AqLife.Application.Business.Tag.Handler
{
    public class TagQueryHandler(TagSearch search, PageResultMapper<TagEntity,TagDto> mapper) : IRequestHandler<TagQuery,PageResult<TagDto>?>
    {
        public async Task<PageResult<TagDto>?> Handle(TagQuery query, CancellationToken ct)
        {
            var entites = await search.SearchPageAsync(query, ct);
            return mapper.ToDto(entites);
        }
    }
}
