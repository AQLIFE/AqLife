using MyLife.Shared.Contracts;

namespace MyLife.Service.ServiceInterfaces
{
    public interface IJwtProvider<T> where T : IUserEntity
    {
        string CreateToken(T account);
        bool IsUserExistsInStorage();
        bool Validate(string name, string pwd);
    }

    public interface IJwtAsyncProvider<T> where T : IUserEntity
    {
        Task<string> CreateTokenAsync(T account);
        Task<bool> ValidateTokenAsync(string token);
    }
}
