using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class AllTodoSearchStrategy
    : AllSearchStrategyBase<TodoEntity, EntitySearchCriteria>
    {
        public override async Task<IEnumerable<TodoEntity>> ExecuteAsync(
            IQueryable<TodoEntity> queryable,
            EntitySearchCriteria criteria,
            CancellationToken ct = default)
        {
            queryable = queryable.Where(e => e.Status != TodoStatus.Completed);
            return await queryable.ToListAsync(ct);
        }
    }
}
