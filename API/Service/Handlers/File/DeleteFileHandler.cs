using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.File
{
    public class DeleteFileHandler(FileService service) : IRequestHandler<DeleteFileCommand,Unit>
    {
        public async Task<Unit> Handle(DeleteFileCommand command, CancellationToken ct)
        {
            await service.TryDeleteAsync([command.UID], ct);
            return Unit.Value;
        }
    }
}
