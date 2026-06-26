
//using MediatR;
//using MyLife.Shared.Command;

//namespace MyLife.Shared.Validator;

//public abstract class AbstractValidator<TResponse> : IValidator<TResponse>
//{
//    private protected abstract string ErrorMessage { init; get; }
//    /// <summary>
//    /// return 返回为 false 时,意为触发验证策略拦截,不允许继续往下执行
//    /// </summary>
//    /// <param name="source"></param>
//    /// <returns></returns>
//    private protected abstract Task<bool> IsValidAsync(TResponse source, CancellationToken ct);
//    public async Task VerifyAsync(TResponse source, CancellationToken ct = default) { if (!await this.IsValidAsync(source, ct)) throw new Exception(this.ErrorMessage); }// 初始逻辑占位

//}
