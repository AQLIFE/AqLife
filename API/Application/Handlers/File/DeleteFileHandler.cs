using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.File
{
    public class DeleteFileHandler(FileService service) : IRequestHandler<DeleteFileCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteFileCommand command, CancellationToken ct)
        {
            await service.TryDeleteAsync([command.UID], ct);
            return Unit.Value;
        }
    }
}
