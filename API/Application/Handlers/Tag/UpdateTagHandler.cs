using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Application.Handlers.Tag
{
    public class UpdateTagHandler(TagServices services) : IRequestHandler<UpdateTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTagCommand command, CancellationToken ct)
            => await services.TryUpdateAsync(ct, command.UID,command.TagName, command.AliasName, command.IsCategory);
    }
}
