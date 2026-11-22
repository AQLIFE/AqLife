using AqLife.Application.File;
using AqLife.Application.File.Service;
using AqLife.Shared.Options;
using MediatR;

namespace AqLife.Application.File.Handler
{
    public class DownloadFileHandler(FileReader fileReader) : IRequestHandler<DownloadFileQuery, FileDownloadModel>
    {
        public async Task<FileDownloadModel> Handle(DownloadFileQuery query, CancellationToken ct)
        => await fileReader.ReadAsync(query.UID, FileScope.Blog, ct);
    }

    public class PreviewFileHandler(FileReader fileReader) : IRequestHandler<PreviewFileQuery, FilePreviewModel>
    {
        public async Task<FilePreviewModel> Handle(PreviewFileQuery query, CancellationToken ct)
            => await fileReader.ReadAsync(query.UID, query.IsTemplate ? FileScope.Template : FileScope.Default, ct);

    }
}
