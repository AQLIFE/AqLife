using MediatR;
using MyLife.Data.Repository;
using MyLife.Shared;

namespace MyLife.Service.Behaviors
{
    // 事务管道
    public class TransactionBehavior<TRequest, TResponse>(AppStorage storage) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 💡 核心强制逻辑：判断当前的请求是否继承了 IRequireTransaction 标签
            if (request is IRequireTransaction)
            {
                // 如果是新增/更新，强制开启事务
                using var transaction = await storage.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next(); // 执行真正的业务
                    await storage.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken); // 提交
                    return response;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken); // 异常回滚
                    throw; // 抛出给你的全局错误捕获器
                }
            }

            // 如果是查询/删除（没有标签），直接执行，不开启事务
            return await next();
        }
    }
}
