using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Search.Base;
using MyLife.Service.Search.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Search.Tag
{
    public class FilterTagSearchStrategy : FilteredSearchStrategyBase<TagEntity, EntitySearchCriteria>
    {
        protected override IQueryable<TagEntity> ApplyKeywordFilter(
        IQueryable<TagEntity> q, string keyword)
        => q.Where(e => e.Name.Contains(keyword));
    }
}
