using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.Accident;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using MyLife.Shared.Tools;

namespace MyLife.Web.Controllers
{
    [ApiController, Route("[controller]")]
    public class AccountController(
        //AccountService service,
        //AccountMapper accountMapper,
        //IOptions<JwtOption> option,
        //IJwtProvider<AccountEntity> jwtProvider 
        IMediator mediator
        ) : ControllerBase
    {
        //[HttpGet, AllowAnonymous]
        //public async Task<AccountDto?> GetValid()
        //=> await service.TryReadAsync() is AccountEntity account ? accountMapper.ToDto(account) : null;

        [HttpGet, AllowAnonymous]
        public async Task<AccountDto?> GetValid()
        => await mediator.Send(new GetAccountQuery());

        [HttpPost, AllowAnonymous]
        public async Task<Guid?> AddAccount([FromForm] CreateAccountCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpPost("login"), AllowAnonymous]
        public async Task<string> Login(LoginCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);


        [HttpPatch, Authorize]
        public async Task<Guid> UpdateAccount([FromForm] UpdateAccountAvatarCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpDelete, Authorize]
        public async Task DeleteAccount(CancellationToken ct)
            => await mediator.Send(new DeleteAccountCommand(User.GetAccountId()), ct);
    }
}
