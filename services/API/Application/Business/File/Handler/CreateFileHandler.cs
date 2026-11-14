using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class CreateFileHandler(FileWriter fileWriter) : IRequestHandler<CreateFileCommand, Guid>,IRequestHandler<UploadFilesCommand,IEnumerable<Guid>>
    {
        public async Task<Guid> Handle(CreateFileCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command, ct);

        public async Task<IEnumerable<Guid>> Handle(UploadFilesCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command,ct);
    }
}
