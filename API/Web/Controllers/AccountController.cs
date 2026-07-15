using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Application.Command;
using MyLife.Shared.Contracts;
using MyLife.Shared.DTOs;
using MyLife.Shared.Tools;

namespace MyLife.Web.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<AccountDto?> GetValid(CancellationToken ct)
        => await mediator.Send(new AccountQuery(), ct);

        [HttpPost, AllowAnonymous]
        public async Task<Guid?> AddAccount(CreateAccountCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpPost("login"), AllowAnonymous]
        public async Task<string> Login(LoginCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);


        [HttpPatch("avatar")]//可能需要传递头像ID?
        public async Task<Guid> UpdateAvatar(IFormFile avatar, CancellationToken ct)
            => await mediator.Send(new UpdateAccountAvatarCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"), avatar), ct);
        [HttpPatch("profile")]
        public async Task<Guid> UpdateProfile(AccountProfile info, CancellationToken ct)
            => await mediator.Send(new UpdateAccountProfileCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"),Name: info.Name,Desc: info.Desc), ct);
        [HttpPut("subscriptions")]// 待定,需要前端验证
        public async Task<Guid> UpdateSubscriptions(IEnumerable<SubscriptionDto> dtos, CancellationToken ct)
            => await mediator.Send(new UpdateAccountSubscriptionsCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"), dtos), ct);

        [HttpDelete]
        public async Task DeleteAccount(CancellationToken ct)
            => await mediator.Send(new DeleteAccountCommand(User.TryGetAccountId()!.Value), ct);
    }
}
