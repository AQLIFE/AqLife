using MediatR;
using MyLife.Application.Command;
using MyLife.Data.Repository;
using MyLife.Service.EntityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Application.Handlers.File
{
    public class UpdateFileTagHandler(FileService service):IRequestHandler<UpdateFileTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileTagCommand command, CancellationToken ct)
        => await service.TryUpdateAsync(command.UID, command.tags, ct);
    }
}
