using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Account.Handler
{
    // 解耦 LoginHandler 与 Service 业务类,通过Search 业务类来获取数据,并通过 IJwtProvider 来生成 Token
    public class LoginHandler(ITokenProvider<AccountEntity> provider, ISearch<AccountQuery, AccountDto, AccountEntity> search) : IRequestHandler<LoginCommand, string>
    {
        public async Task<string> Handle(LoginCommand command, CancellationToken ct)
        {
            var entity = await search.SearchAsync(new AccountQuery(), ct);
            return entity.Count() > 0 && entity.First() is AccountEntity account ? provider.CreateToken(account) : throw new AuthenticationException("授权失败");
        }
        // 经过业务检查,基本不可能存在空
    }
}
