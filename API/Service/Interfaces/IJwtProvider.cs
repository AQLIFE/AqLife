using MyLife.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Interfaces
{
    public interface IJwtProvider<T> where T: IBaseUser
    {
        string CreateToken(T account);
        (bool IsValid, T? account) ValidateToken(string token);
    }

    public interface IJwtAsyncProvider<T> where T : IBaseUser
    {
        Task<string> CreateTokenAsync(T account);
        Task<(bool IsValid,T? account)> ValidateTokenAsync(string token);
    }
}
