using MyLife.Domain.Contracts;
using System.Security.Claims;

namespace MyLife.Service.Interfaces
{
    public interface IJwtProvider<T> where T : IUserEntity
    {
        string CreateToken(T account);
        bool IsUserExistsInStorage(ClaimsPrincipal principal);
        bool Validate(string name, string pwd);
    }

    public interface IJwtAsyncProvider<T> where T : IUserEntity
    {
        Task<string> CreateTokenAsync(T account);
        Task<bool> ValidateTokenAsync(string token);
    }
}
