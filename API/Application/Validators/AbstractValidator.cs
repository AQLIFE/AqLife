using MyLife.Shared.Contracts;
using MyLife.Shared.Exceptions;

namespace MyLife.Application.Validators;

public abstract class AbstractValidator<TResponse> : IValidator<TResponse>
{
    private protected abstract string ErrorMessage { init; get; }
    /// <summary>
    /// return 返回为 false 时,意为触发验证策略拦截,不允许继续往下执行
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private protected abstract Task<bool> IsValidAsync(TResponse source, CancellationToken ct);
    /// <summary>
    /// 定义该验证器触发时抛出的异常类型，默认是业务逻辑检查异常 [cite: 55]
    /// </summary>
    private protected virtual BusinessException CreateException(string message)
        => new RequestCheckException(message);
    public async Task VerifyAsync(TResponse source, CancellationToken ct = default) { if (!await this.IsValidAsync(source, ct)) throw CreateException(this.ErrorMessage); }// 初始逻辑占位

}
