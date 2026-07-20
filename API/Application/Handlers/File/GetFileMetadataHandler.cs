using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.File
{
    public class GetFileMetadataHandler(FileService service, FileMapper mapper) : IRequestHandler<GetFileMetadataQuery, IEnumerable<FileDto>>
    {
        public async Task<IEnumerable<FileDto>> Handle(GetFileMetadataQuery query, CancellationToken ct)
        {
            var result = await service.TryReadAsync(ct, query.UID, query.Title);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
