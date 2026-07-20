using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Tag
{
    public class DeleteTagHandler(TagServices services): IRequestHandler<DeleteTagCommand,Unit>
    {
        public async Task<Unit> Handle(DeleteTagCommand command, CancellationToken ct)
        {
            await services.TryDeleteAsync(command.UID, ct);
            return Unit.Value;
        }

    }
}
