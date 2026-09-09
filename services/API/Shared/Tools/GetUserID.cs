using AqLife.Shared.Exceptions;
using Duende.IdentityModel;
using System.Security.Claims;

namespace AqLife.Shared.Tools
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? TryGetAccountId(this ClaimsPrincipal user)
        {
            // 注意：如果你创建时用的是 JwtClaimTypes.Id，对应的 ClaimType 字符串通常是 "id" 或 ClaimTypes.NameIdentifier
            // 可以直接使用你创建 Token 时填入的字符串
            var idStr = user.FindFirst(JwtClaimTypes.Id)?.Value ?? throw new AuthenticationException("token 无效");

            if (Guid.TryParse(idStr, out var guid))
            {
                return guid;
            }
            return null;
        }
    }
}
