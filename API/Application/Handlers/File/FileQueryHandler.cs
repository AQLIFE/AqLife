using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Service.Search.File;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.File
{
    public class FileQueryHandler(FileSearch search, FileMapper mapper) : IRequestHandler<FileQuery, IEnumerable<FileDto>>
    {
        public async Task<IEnumerable<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(query,ct);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
