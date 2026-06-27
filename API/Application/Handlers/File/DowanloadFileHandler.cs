using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;
using MyLife.Shared.DTOs;

namespace MyLife.Application.Handlers.File
{
    public class DownloadFileHandler(FileService service) : IRequestHandler<DownloadFileQuery, FileDownloadModel>
    {
        public async Task<FileDownloadModel> Handle(DownloadFileQuery query, CancellationToken ct)
        => await service.GetFileInternalAsync(query.UID, ct);
    }
}
