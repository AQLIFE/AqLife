using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
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
        AccountService service,
        AccountMapper accountMapper,
        IOptions<JwtOption> option,
        AppStorage storage,
        IJwtProvider<AccountEntity> jwtProvider 
        ) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<AccountDto?> GetValid()
        => await service.TryReadAsync() is AccountEntity account ? accountMapper.ToDto(account) : null;

        [HttpPost, AllowAnonymous]
        public async Task<Guid?> AddAccount(AccountDto dto, string? SecretKey = null)
        {
            if (SecretKey is string str && str == option.Value.SecretKey)
            {
                var account =  await service.TryCreateAsync(dto);
                await storage.SaveChangesAsync();
                return account.UID;
            }
            else throw new OperateAuthorizationException("不允许的操作");
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<string> Login(LoginDto dto)
        {
            if (await service.TryLogin(dto) && await service.TryReadAsync() is AccountEntity account && account.Name == dto.Name && account.IsValid)
            {
                HttpContext.Response.Headers.Append("Authorization", jwtProvider.CreateToken(account));
                return "登录成功";
            }
            return "登录失败";
        }

        [HttpPatch, Authorize]
        public async Task<Guid> UpdateAccount(AccountDto dto)
            => await service.TryUpdateAsync(dto, User.GetAccountId());

        [HttpDelete, Authorize]
        public async Task<int> DeleteAccount()
            => await service.TryDeleteAsync(User.GetAccountId());
    }
}
