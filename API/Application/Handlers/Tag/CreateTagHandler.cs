using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Tag
{
    public class CreateTagHandler(TagServices services) : IRequestHandler<CreateTagCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTagCommand command, CancellationToken ct)
            => await services.TryCreateAsync(ct, command.TagName, command.AliasName, command.IsCategory);
    }
}
