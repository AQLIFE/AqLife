using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.File
{
    public class UpdateFileHandler(FileService service) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.File, ct);
    }
}
