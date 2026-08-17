using MyLife.Domain.Contracts;
using System.Security.Claims;

namespace MyLife.Application.Abstractions.Authentication
{
    public interface ITokenProvider<in T> where T : IUserEntity
    {
        string CreateToken(T account);
        Task<bool> IsUserExistsAsync(
        ClaimsPrincipal principal,
        CancellationToken ct = default);
    }

}
