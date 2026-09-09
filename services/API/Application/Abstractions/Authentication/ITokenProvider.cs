using AqLife.Domain.Contracts;
using System.Security.Claims;

namespace AqLife.Application.Abstractions.Authentication
{
    public interface ITokenProvider<in T> where T : IUserEntity
    {
        string CreateToken(T account);
        Task<bool> IsUserExistsAsync(
        ClaimsPrincipal principal,
        CancellationToken ct = default);
    }

}
