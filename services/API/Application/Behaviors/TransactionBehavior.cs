using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Behaviors
{
    // 事务管道
    public class TransactionBehavior<TRequest, TResponse>(IApplicationDbContext storage, IFileStorage fileStorage, UploadContext uploadContext) : IPipelineBehavior<TRequest, TResponse>
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
                if (request is IRequireDependencyVersionIncrement devPlan)
                {
                    DevelopmentPlanEntity developmentPlanEntity = await storage.DevelopmentPlans.SingleAsync(e => e.UID == devPlan.PlanId, cancellationToken);
                    developmentPlanEntity.IncrementDependencyVersion();
                }
                await storage.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken); // 提交 : 隐式事务回滚机制,若在Commit之前触发异常,则会自动回滚
                foreach (var path in uploadContext.DeletePath)
                    await fileStorage.DeleteAsync(path, cancellationToken);
                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ResourceConflictException("数据已被其他请求修改，请刷新后重试");
            }
            catch
            {
                using var cleanupCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await transaction.RollbackAsync(cancellationToken);
                foreach (var path in uploadContext.SavePath)
                    await fileStorage.DeleteAsync(path, cleanupCts.Token);
                throw;
            }

        }
    }
}
