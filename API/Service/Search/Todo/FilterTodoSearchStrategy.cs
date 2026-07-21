using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Search.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Search.Todo
{
    
    public class FilterTodoSearchStrategy : FilteredSearchStrategyBase<TodoEntity, EntitySearchCriteria>
    {
        protected override IQueryable<TodoEntity> ApplyKeywordFilter(
        IQueryable<TodoEntity> q, string keyword)
        => q.Where(e => e.Desc.Contains(keyword));
    }
}
