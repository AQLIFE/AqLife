using MyLife.Shared.DTOs;

namespace MyLife.Service.Interfaces
{
    public interface IAccountStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>IsValid 为 true 时有效</returns>
        /// <returns>Message 当 IsValid == false 时返回的具体原因</returns>
        (bool IsValid, string Message) Check(AccountDto account);
    }
}
