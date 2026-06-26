using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.DTOs;

namespace MyLife.Service.Handlers.File
{
    public class GetFileMetadataHandler(FileService service, FileMapper mapper) : IRequestHandler<GetFileMetadataQuery, IEnumerable<FileMetadataDto>>
    {
        public async Task<IEnumerable<FileMetadataDto>> Handle(GetFileMetadataQuery query, CancellationToken ct)
        {
            var result = await service.TryReadAsync(ct, query.UID, query.Title);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
