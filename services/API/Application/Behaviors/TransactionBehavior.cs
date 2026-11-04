using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.CommandInterface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Behaviors
{
    // 事务管道
    public class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext storage) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 💡 核心强制逻辑：判断当前的请求是否继承了 IRequireTransaction 标签
            if (request is not IRequireTransaction)
                return await next(cancellationToken);

            await using var transaction = await storage.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next(cancellationToken); // 执行真正的业务
                await storage.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken); // 提交 : 隐式事务回滚机制,若在Commit之前触发异常,则会自动回滚
                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ResourceConflictException("数据已被其他请求修改，请刷新后重试");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }
    }
}
