using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Pipeline.Behaviors
{
    /// <summary>
    /// Owns the application transaction and file-storage compensation lifecycle.
    /// Business-module-specific rules are implemented by behaviors in their own modules.
    /// </summary>
    public class TransactionBehavior<TRequest, TResponse>(
        IApplicationDbContext dbContext,
        IFileStorage fileStorage,
        UploadContext uploadContext,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not IRequireTransaction)
                return await next(cancellationToken);

            await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);
            try
            {
                // Inner module behaviors and the handler execute before this transaction commits.
                var response = await next(cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                foreach (var path in uploadContext.DeletePath)
                    await fileStorage.DeleteAsync(path, cancellationToken);

                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ResourceConflictException("数据已被其他请求修改，请刷新后重试");
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("保存的数据不符合业务规范: {$0}", ex.InnerException?.Message);
                await transaction.RollbackAsync(cancellationToken);
                throw new ResourceConflictException("数据不符合业务规范");
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