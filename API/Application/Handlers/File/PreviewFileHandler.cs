using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;
using MyLife.Shared.DTOs;

namespace MyLife.Application.Handlers.File
{
    public class PreviewFileHandler(FileService service) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
        => await service.GetFileInternalAsync(query.UID, ct);
    }
}
