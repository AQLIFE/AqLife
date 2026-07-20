using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.File
{
    public class DownloadFileHandler(FileService service) : IRequestHandler<DownloadFileQuery, FileDownloadModel>
    {
        public async Task<FileDownloadModel> Handle(DownloadFileQuery query, CancellationToken ct)
        => await service.GetFileInternalAsync(query.UID, ct);
    }
}
