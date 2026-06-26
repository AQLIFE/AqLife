using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.File
{
    public class UpdateFileHandler(FileService service) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.File, ct);
    }
}
