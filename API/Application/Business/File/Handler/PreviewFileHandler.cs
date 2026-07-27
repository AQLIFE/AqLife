using MediatR;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.File.Handler
{
    public class PreviewFileHandler(FileReader fileReader) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
        => await fileReader.ReadAsync(query.UID, ct);
    }
}
