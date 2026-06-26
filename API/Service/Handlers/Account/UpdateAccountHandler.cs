using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.Account
{
    public class UpdateAccountProfileHandler(AccountService service) : IRequestHandler<UpdateAccountProfileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        => await service.TryUpdateAsync(command.UID, command.Desc, command.Name, ct);
    }

    public class UpdateAccountAvatarHandler(AccountService service) : IRequestHandler<UpdateAccountAvatarCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.Avatar, ct);
    }

    public class UpdateAccountSubscriptionsHandler(AccountService service) : IRequestHandler<UpdateAccountSubscriptionsCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
            => await service.TryUpdateAsync(command.UID, command.Subscriptions, ct);
    }
}
