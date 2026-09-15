using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class PreviewFileHandler(FileReader fileReader) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
            => await fileReader.ReadAsync(query.UID, ct);

    }
}
