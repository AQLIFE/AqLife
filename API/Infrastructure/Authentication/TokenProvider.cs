using Duende.IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLife.Application.Abstractions.Authentication;
using MyLife.Domain.Entities;
using MyLife.Shared.Options;
using MyLife.Shared.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyLife.Infrastructure.Authentication
{
    public class TokenProvider(IOptions<JwtOption> options,AppStorage appStorage) : ITokenProvider<AccountEntity>
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
        public async Task<bool> IsUserExistsAsync(ClaimsPrincipal principal,CancellationToken ct = default)
        {
            if (principal?.Identity?.IsAuthenticated != true)
                return false;


            var uid = principal.TryGetAccountId();


            if (uid == Guid.Empty)
                return false;


            return await appStorage.Accounts.AsNoTracking().AnyAsync(e => e.UID == uid&& e.IsValid,ct);
        }
    }
}
