using MediatR;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;

namespace MyLife.Application.Business.File.Handler
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
