using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;


namespace MyLife.Application.Business.File.Handler
{
    public class UpdateFileTagHandler(FileService service) : IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        => await service.TryUpdateAsync(command.UID, command.tags, ct);
    }
}
