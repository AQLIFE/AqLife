using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.File
{
    public class CreateFileHandler(FileService service) : IRequestHandler<CreateFileCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(CreateFileCommand command, CancellationToken ct)
        => await service.TryCreateAsync(command.File, ct);
    }
}
