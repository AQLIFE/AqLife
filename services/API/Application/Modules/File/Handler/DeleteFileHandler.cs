using File;
using File.Service;
using MediatR;

namespace File.Handler
{
    public class DeleteFileHandler(FileDeleter fileDeleter) : IRequestHandler<DeleteFileCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteFileCommand command, CancellationToken ct)
        {
            await fileDeleter.DeleteAsync([command.UID], ct);
            return Unit.Value;
        }
    }
}
