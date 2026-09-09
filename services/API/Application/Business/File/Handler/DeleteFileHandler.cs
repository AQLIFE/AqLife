using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.File.Handler
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
