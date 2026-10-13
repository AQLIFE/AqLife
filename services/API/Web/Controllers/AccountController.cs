
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous,EndpointDescription("返回一个有效的账户,可能为 null")]
        public async Task<AccountDto?> GetValid(CancellationToken ct)
        => await mediator.Send(new AccountQuery(), ct);

        [HttpPost, AllowAnonymous,EndpointDescription("注册账户成功后返回唯一登录账户名")]
        public async Task<string> AddAccount(CreateAccountCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpPost("login"), AllowAnonymous,EndpointDescription("登录账户成功后返回 JWT Beearer Token")]
        public async Task<string> Login(LoginCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);


        [HttpPatch("avatar"),EndpointDescription("发送一个图像用以更新账户头像,返回 Guid")]
        public async Task<string> UpdateAvatar(IFormFile avatar, CancellationToken ct)
            => await mediator.Send(new UpdateAccountAvatarCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"), avatar), ct);
        [HttpPatch("profile"),EndpointDescription("更新账户信息(包含公开名称,BIO);返回账户ID")]
        public async Task<string> UpdateProfile(AccountProfile info, CancellationToken ct)
            => await mediator.Send(new UpdateAccountProfileCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"), Name: info.Name, Desc: info.Desc), ct);
        [HttpPut("subscriptions"),EndpointDescription("更新账户个人主页列表,返回账户ID")]// 待定,需要前端验证
        public async Task<string> UpdateSubscriptions(IEnumerable<SubscriptionDto> dtos, CancellationToken ct)
            => await mediator.Send(new UpdateAccountSubscriptionsCommand(User.TryGetAccountId() ?? throw new ArgumentNullException("无法识别的ID"), dtos), ct);

        [HttpDelete,EndpointDescription("删除指定ID的账户,无返回")]
        public async Task DeleteAccount(CancellationToken ct)
            => await mediator.Send(new DeleteAccountCommand(User.TryGetAccountId()!.Value), ct);
    }
}
