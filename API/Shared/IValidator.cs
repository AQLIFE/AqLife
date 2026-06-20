using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared
{
    public interface IValidator<T>
    {
        //private protected string ErrorMessage { init; get; }
        /// <summary>
        /// return 返回为真时,意为触发验证策略拦截,不允许继续往下执行
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        //private protected bool Validator(T source);
        public Task VerifyAsync(T source, CancellationToken ct = default);
    }
}
