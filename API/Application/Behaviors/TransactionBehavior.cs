using MediatR;
using MyLife.Data.Repository;
using MyLife.Shared.Command;

namespace MyLife.Application.Behaviors
{
    // 事务管道
    public class TransactionBehavior<TRequest, TResponse>(AppStorage storage) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 💡 核心强制逻辑：判断当前的请求是否继承了 IRequireTransaction 标签
            if (request is not IRequireTransaction)
                return await next();


            using var transaction = await storage.Database.BeginTransactionAsync(cancellationToken);
            var response = await next(); // 执行真正的业务
            await storage.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken); // 提交 : 隐式事务回滚机制,若在Commit之前触发异常,则会自动回滚
            return response;

        }
    }
}
