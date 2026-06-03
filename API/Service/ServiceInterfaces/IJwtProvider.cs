using MyLife.Shared;

namespace MyLife.Service.ServiceInterfaces
{
    public interface IJwtProvider<T> where T : IBaseUser
    {
        string CreateToken(T account);
        bool ValidateToken(string token);
    }

    public interface IJwtAsyncProvider<T> where T : IBaseUser
    {
        Task<string> CreateTokenAsync(T account);
        Task<bool> ValidateTokenAsync(string token);
    }
}
