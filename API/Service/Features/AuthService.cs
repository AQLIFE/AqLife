using Duende.IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.Options;
using MyLife.Shared.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyLife.Service.Features
{
    public class AuthService(IOptions<JwtOption> options, AppStorage storage, IHttpContextAccessor httpContextAccessor) : IJwtProvider<AccountEntity>
    {
        public string CreateToken(AccountEntity account)
        {
            var claims = new[]
            {
            new Claim(JwtClaimTypes.Id, account.UID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var sourceToken = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(sourceToken);
        }

        public bool Validate(string name, string pwd)
        {
            var isValid = storage.Account.Any(e => e.Name == name);
            var isAuth = options.Value.SecretKey == pwd;
            return isAuth && isValid;
        }

        public bool IsUserExistsInStorage(ClaimsPrincipal principal)
        {
            var context = httpContextAccessor.HttpContext;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return false; // 用户根本没登录/Token无效
            }

            // 从合法的 ClaimsPrincipal 中提取 UID
            var uid = principal.TryGetAccountId();

            // 检查数据库中该用户是否依然合法存在（比如防止用户被中途注销或删除）
            return storage.Account.Any(e => e.UID == uid);
        }
    }
}
