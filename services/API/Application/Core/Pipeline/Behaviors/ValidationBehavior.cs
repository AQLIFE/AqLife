using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.Contracts;
using MediatR;

namespace Core.Pipeline.Behaviors;

// 统一的检查管道（任何请求进来，先强制走这里）
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, IApplicationRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // 💡 异步执行每一个验证器
        foreach (var validator in validators)
        {
            await validator.VerifyAsync(request, ct);
        }

        // 💡 所有校验通过后，再进入真正的业务逻辑 (Handler)
        return await next(ct);
    }
}
