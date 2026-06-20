using MediatR;
using MyLife.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Behaviors
{
    // 统一的检查管道（任何请求进来，先强制走这里）
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            // 💡 异步执行每一个验证器
            foreach (var validator in validators)
            {
                // 如果其中一个是 FileHashUniqueValidator，它会执行异步哈希计算和 DB 查询
                await validator.VerifyAsync(request, ct);
            }

            // 💡 所有校验通过后，再进入真正的业务逻辑 (Handler)
            return await next();
        }
    }
}
