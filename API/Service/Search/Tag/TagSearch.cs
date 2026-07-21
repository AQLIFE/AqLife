using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Mapper;
using MyLife.Service.Search.Base;
using MyLife.Shared.IView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Search.Tag
{
    public class TagSearch(QueryMapper queryMapper, AppStorage storage
    , IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, TagEntity, TagDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TagEntity>> BuildBaseQueryAsync(IQueryable<TagEntity> queryable,TagQuery query)
       => queryable;
    }
}
