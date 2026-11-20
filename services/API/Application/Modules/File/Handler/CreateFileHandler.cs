using AqLife.Application.Modules.File.Service;
using MediatR;

namespace AqLife.Application.Modules.File.Handler
{
    public class CreateFileHandler(FileWriter fileWriter) : IRequestHandler<CreateFileCommand, Guid>, IRequestHandler<UploadFilesCommand, IEnumerable<Guid>>
    {
        public async Task<Guid> Handle(CreateFileCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command, ct);

        public async Task<IEnumerable<Guid>> Handle(UploadFilesCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command, ct);
    }
}
