using MediatR;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.File.Handler
{
    public class DownloadFileHandler(FileReader fileReader) : IRequestHandler<DownloadFileQuery, FileDownloadModel>
    {
        public async Task<FileDownloadModel> Handle(DownloadFileQuery query, CancellationToken ct)
        => await fileReader.ReadAsync(query.UID, ct);
    }
}
