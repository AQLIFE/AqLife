using MediatR;
using MyLife.Application.Abstractions.Authentication;
using MyLife.Application.Business.Account.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;

namespace MyLife.Application.Business.Account.Handler
{
    // 解耦 LoginHandler 与 Service 业务类,通过Search 业务类来获取数据,并通过 IJwtProvider 来生成 Token
    public class LoginHandler(ITokenProvider<AccountEntity> provider, AccountSearch search) : IRequestHandler<LoginCommand, string>
    {
        public async Task<string> Handle(LoginCommand command, CancellationToken ct)
        {
            var entity = await search.SearchAsync(new AccountQuery(), ct);
            return entity.Count() > 0 && entity.First() is AccountEntity account ? provider.CreateToken(account) : throw new AuthenticationException("授权失败");
        }
        // 经过业务检查,基本不可能存在空
    }
}
