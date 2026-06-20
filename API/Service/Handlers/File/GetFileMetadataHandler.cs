using MediatR;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Implementations;
using MyLife.Service.Mappings;
using MyLife.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.File
{
    public class GetFileMetadataHandler(FileService service,FileMapper mapper) : IRequestHandler<GetFileMetadataQuery, IEnumerable<FileMetadataDto>>
    {
        public async Task<IEnumerable<FileMetadataDto>> Handle(GetFileMetadataQuery query, CancellationToken ct)
        {
            var result = await service.TryReadAsync(ct,query.UID,query.Title);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
