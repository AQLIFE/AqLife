using Duende.IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLife.Domain.Entities;
using MyLife.Shared.Options;
using System.Security.Claims;
using System.Text;

namespace MyLife.Infrastructure.Authentication
{
    public class TokenProvider(IOptions<JwtOption> options) : ITokenProvider<AccountEntity>
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
    }
}
