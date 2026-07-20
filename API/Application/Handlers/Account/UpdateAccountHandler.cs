using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Account
{
    public class UpdateAccountProfileHandler(AccountService service) : IRequestHandler<UpdateAccountProfileCommand, string>
    {
        public async Task<string> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        => await service.TryUpdateAsync(command.UID,name: command.Name,desc: command.Desc, ct);
    }

    public class UpdateAccountAvatarHandler(AccountService service) : IRequestHandler<UpdateAccountAvatarCommand, string>
    {
        public async Task<string> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.Avatar, ct);
    }

    public class UpdateAccountSubscriptionsHandler(AccountService service) : IRequestHandler<UpdateAccountSubscriptionsCommand, string>
    {
        public async Task<string> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.Subscriptions, ct);
    }
}
