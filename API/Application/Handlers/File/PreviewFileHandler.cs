using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.File
{
    public class PreviewFileHandler(FileService service) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
        => await service.GetFileInternalAsync(query.UID, ct);
    }
}
