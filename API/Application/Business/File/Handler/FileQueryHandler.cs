using MediatR;
using MyLife.Application.Business.File.Search;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.File.Handler
{
    public class FileQueryHandler(FileSearch search, FileMapper mapper) : IRequestHandler<FileQuery, IEnumerable<FileDto>>
    {
        public async Task<IEnumerable<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(query, ct);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
