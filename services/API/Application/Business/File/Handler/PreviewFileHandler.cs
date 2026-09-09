using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class PreviewFileHandler(FileReader fileReader, PreviewContext previewContext) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
        {
            previewContext.IsPreview = true;
            return await fileReader.ReadAsync(query.UID, ct);
        }
    }
}
