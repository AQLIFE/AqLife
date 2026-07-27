using MyLife.Domain.Contracts;

namespace MyLife.Application.Abstractions.Authentication
{
    public interface ITokenProvider<in T> where T : IUserEntity
    {
        string CreateToken(T account);
    }
}
