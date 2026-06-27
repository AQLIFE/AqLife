using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;
using MyLife.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
