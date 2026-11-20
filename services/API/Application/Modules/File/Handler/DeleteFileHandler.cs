using AqLife.Application.Modules.File.Service;
using MediatR;

namespace AqLife.Application.Modules.File.Handler
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
