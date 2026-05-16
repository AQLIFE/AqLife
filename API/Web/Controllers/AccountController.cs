using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Implementations;
using MyLife.Service.Interfaces;
using MyLife.Service.Mappings;
using MyLife.Shared.Accident;
using MyLife.Shared.Options;
using MyLife.Shared.DTOs;

namespace MyLife.Web.Controllers
{
    [ApiController, Route("[controller]")]
    public class AccountController(
        AppStorage storage,
        AccountService service,
        IGenericsMapper<AccountEntity,AccountDto> accountMapper,
        IOptions<JwtOption> option,
        IJwtProvider<AccountEntity> jwtProvider
        //IGenericsMapper<SubscriptionEntity, SubscriptionDto> subscriptionMapper
        ) : ControllerBase
    {
        [HttpGet,AllowAnonymous]
        public async Task<AccountDto?> GetValid()
        => storage.Account.Include(e=>e.Subscriptions).AsNoTracking().OrderBy(e => e.Name).Where(e => e.IsValid == true).FirstOrDefault() is AccountEntity account
                ? accountMapper.Desensitization(account)
                : null;

        [HttpPost,AllowAnonymous]
        public async Task<string> AddAccount(string name,string? desc=null,string? SecretKey = null)
            => SecretKey is string str && str == option.Value.SecretKey
                ? await service.Init(name,desc)
                : throw new OperateAuthorizationException("不允许的操作");

        [HttpPost("subscription"), Authorize]
        public async Task<int> AddSubscription(params SubscriptionDto[] subscriptionDtos)
            => await service.AddSubscriptionAsync(subscriptionDtos);

        [HttpPost("login"),AllowAnonymous]
        public async Task<string> Login(LoginDto dto)
        {
            if (dto.Password == option.Value.SecretKey && storage.Account.Where(e => e.Name == dto.Name).FirstOrDefault() is AccountEntity account)
            {
                HttpContext.Response.Headers.Append("Authorization", jwtProvider.CreateToken(account));
                return "登录成功";
            }
            else
            {
                return "登录失败";
            }
        }


    }
}
