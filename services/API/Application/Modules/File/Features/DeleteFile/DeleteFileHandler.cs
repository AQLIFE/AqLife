using AqLife.Application.File;
using AqLife.Application.File.Service;
using MediatR;

namespace AqLife.Application.File.Handler
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
