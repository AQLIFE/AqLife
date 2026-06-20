using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Implementations;
using MyLife.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.File
{
    public class PreviewFileHandler(FileService service) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
        => await service.GetFileInternalAsync(query.UID,ct);
    }
}
