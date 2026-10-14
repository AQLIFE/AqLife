using AqLife.Domain.Contracts;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace AqLife.CoreTest.IntegrationTest
{
    public abstract class IntegrationTestBase<TEntity>(IntegrationTestFixture fixture) where TEntity : class, IEntity
    {
        protected IServiceScope CreateScope()
        => fixture.Services.CreateScope();

        protected async Task<string> EnsureSystemKeyAsync()
            => await fixture.Services.EnsureAndGetSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");

        protected async Task<TEntity?> QueryAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            return await storage.Set<TEntity>().SingleOrDefaultAsync(query, cancellationToken);
        }

        protected async Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }


        /// <summary>
        /// 跨实体查询: 当需要检查主实体之外的表(如 Subscription)时使用
        /// </summary>
        protected async Task<TOther?> QueryAsync<TOther>(Expression<Func<TOther, bool>> query, CancellationToken cancellationToken = default)
            where TOther : class, IEntity
        {
            using var scope = CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();
            return await storage.Set<TOther>().SingleOrDefaultAsync(query, cancellationToken);
        }
        /// <summary>
        /// 单实体查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();
            return await storage.Set<TEntity>().AnyAsync(query, cancellationToken);
        }
        /// <summary>
        /// 跨实体存在性检查: 当需要检查主实体之外的表(如 Subscription)时使用
        /// </summary>
        protected async Task<bool> AnyAsync<TOther>(Expression<Func<TOther, bool>> query, CancellationToken cancellationToken = default)
            where TOther : class, IEntity
        {
            using var scope = CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();
            return await storage.Set<TOther>().AnyAsync(query, cancellationToken);
        }
    }
}