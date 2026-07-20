using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Tag
{
    public class UpdateTagHandler(TagServices services) : IRequestHandler<UpdateTagCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTagCommand command, CancellationToken ct)
            => await services.TryUpdateAsync(ct, command.UID,command.TagName, command.AliasName, command.IsCategory);
    }
}
