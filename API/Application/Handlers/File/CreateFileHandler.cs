using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.File
{
    public class CreateFileHandler(FileService service) : IRequestHandler<CreateFileCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(CreateFileCommand command, CancellationToken ct)
        => await service.TryCreateAsync(command.File, ct);
    }
}
