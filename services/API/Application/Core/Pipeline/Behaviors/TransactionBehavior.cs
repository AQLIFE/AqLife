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
        IUnitOfWork unitOfWork,
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

            await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
            TResponse response;
            var commitAttempted = false;

            // Only failures known to occur before commit may compensate newly uploaded files.
            try
            {
                // Inner module behaviors and the handler execute before this transaction commits.
                response = await next(cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                // Once commit starts, its outcome may be ambiguous after a connection failure.
                // Prefer a possible orphaned upload over deleting a file that may now be referenced.
                commitAttempted = true;
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await TryRollbackAsync(transaction, cancellationToken);
                if (!commitAttempted)
                    await CleanupNewUploadsAsync();
                throw new ResourceConflictException("数据已被其他请求修改，请刷新后重试");
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("保存的数据不符合业务规范: {InnerMessage}", ex.InnerException?.Message);
                await TryRollbackAsync(transaction, cancellationToken);
                if (!commitAttempted)
                    await CleanupNewUploadsAsync();
                throw new ResourceConflictException("数据不符合业务规范");
            }
            catch
            {
                await TryRollbackAsync(transaction, cancellationToken);
                if (!commitAttempted)
                    await CleanupNewUploadsAsync();
                else
                    logger.LogError("数据库提交结果不确定，保留本次新上传文件以避免删除可能已被引用的文件"); 
                throw;
            }

            // The database is committed. Old-file cleanup is best-effort and must never
            // enter the transaction-failure compensation path or delete newly saved files.
            foreach (var path in uploadContext.DeletePath)
            {
                try
                {
                    using var cleanupCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    await fileStorage.DeleteAsync(path, cleanupCts.Token);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "数据库事务已提交，但旧文件清理失败，需后续人工检查: {StoragePath}", path);
                }
            }

            return response;
        }

        private async Task TryRollbackAsync(ITransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "数据库事务回滚失败或事务已结束");
            }
        }

        private async Task CleanupNewUploadsAsync()
        {
            using var cleanupCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            foreach (var path in uploadContext.SavePath)
            {
                try
                {
                    await fileStorage.DeleteAsync(path, cleanupCts.Token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "事务失败后清理新上传文件失败: {StoragePath}", path);
                }
            }
        }
    }
}