using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class DownloadFileHandler(FileReader fileReader) : IRequestHandler<DownloadFileQuery, FileDownloadModel>
    {
        public async Task<FileDownloadModel> Handle(DownloadFileQuery query, CancellationToken ct)
        => await fileReader.ReadAsync(query.UID, ct);
    }
}
