using AqLife.Application.Abstractions.Authentication;
using AqLife.Domain.Entities;
using AqLife.Shared.Options;
using AqLife.Shared.Tools;
using Duende.IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;

namespace AqLife.Infrastructure.Authentication
{
    public class TokenProvider(IOptions<JwtOption> options, AppStorage appStorage) : ITokenProvider<AccountEntity>
    {
        public string CreateToken(AccountEntity account)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(options.Value.SecretKey));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var sourceToken = new SecurityTokenDescriptor
            {
                Issuer = options.Value.Issuer,
                Audience = options.Value.Audience,
                Claims = new Dictionary<string, object>
                {
                    [JwtClaimTypes.Id] = account.UID.ToString()
                },
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds
            };

            return new JsonWebTokenHandler().CreateToken(sourceToken);
        }
        public async Task<bool> IsUserExistsAsync(ClaimsPrincipal principal, CancellationToken ct = default)
        {
            if (principal?.Identity?.IsAuthenticated != true)
                return false;


            var uid = principal.TryGetAccountId();


            if (uid == Guid.Empty)
                return false;


            return await appStorage.Accounts.AsNoTracking().AnyAsync(e => e.UID == uid && e.IsValid, ct);
        }
    }
}
