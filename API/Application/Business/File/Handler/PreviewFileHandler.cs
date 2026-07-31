using MediatR;
using MyLife.Application.Business.File.Search;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;
using MyLife.Shared.IView;
using MyLife.Shared.Tools;

namespace MyLife.Application.Business.File.Handler
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
