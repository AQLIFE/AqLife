using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.File
{
    public class CreateFileHandler(FileService service): IRequestHandler<CreateFileCommand,IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(CreateFileCommand command, CancellationToken ct)
        => await service.TryCreateAsync(command.File, ct);
    }
}
