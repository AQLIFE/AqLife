using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace AqLife.CoreTest.IntegrationTest
{
    public abstract class IntegrationTestBase<TEntity>(IntegrationTestFixture fixture) where TEntity : class,IEntity
    {
        protected IServiceScope CreateScope()
        => fixture.Services.CreateScope();

        protected async Task<string?> EnsureSystemKeyAsync()
            => await fixture.Services.EnsureAndGetSystemKeyAsync();

        //protected virtual IntegrationTestFixture InnerFixture => fixture;

        protected async Task<TEntity?> QueryAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)        
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            return await storage.Set<TEntity>().SingleOrDefaultAsync(query,cancellationToken);
        }

        protected async Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request,CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();
            return await storage.Set<TEntity>().AnyAsync(query, CancellationToken.None);
        }
    }
}
