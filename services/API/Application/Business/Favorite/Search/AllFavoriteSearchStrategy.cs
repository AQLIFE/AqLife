using AqLife.Application.Business.File.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Favorite.Search
{
    public sealed class AllFavoriteSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<FavoriteItemEntity, FavoriteSearchCriteria>(httpContext)
    {
        public override bool IsMatch(FavoriteSearchCriteria c)
        => base.IsMatch(c) && c.TagId is null && c.CategoryId is null;
    }
}
