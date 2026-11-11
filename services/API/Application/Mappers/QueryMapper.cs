using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Business.Favorite.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Shared.Contracts;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public partial class QueryMapper :
          ISearchMapper<TodoQuery, TodoSearchCriteria>,
          ISearchMapper<TagQuery, EntitySearchCriteria>,
          //ISearchMapper<AccountQuery, EntitySearchCriteria>,
          ISearchMapper<FileQuery, FileSearchCriteria>,
          ISearchMapper<CorpusQuery, EntitySearchCriteria>,
          ISearchMapper<FavoriteQuery, FavoriteSearchCriteria>,
          ISearchMapper<CategoryQuery, EntitySearchCriteria>
    {
        [MapperIgnoreSource(nameof(TodoQuery.Page))]
        [MapperIgnoreSource(nameof(TodoQuery.PageSize))]
        [MapProperty(nameof(TodoQuery.Title), nameof(TodoSearchCriteria.Keyword))]
        public partial TodoSearchCriteria ToCriteria(TodoQuery query);


        [MapperIgnoreSource(nameof(TagQuery.Page))]
        [MapperIgnoreSource(nameof(TagQuery.PageSize))]
        [MapProperty(nameof(TagQuery.Tag), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TagQuery query);


        [MapProperty(nameof(DevelopmentPlanQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(DevelopmentPlanQuery query);

        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        [MapperIgnoreSource(nameof(TodoRoadmapQuery.PlanId))]
        public partial EntitySearchCriteria ToCriteria(TodoRoadmapQuery query);


        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(AccountQuery query);// 特殊实现,该Query 仅用于查询当前用户,不需要 Keyword, 但仍然使用 EntitySearchCriteria 作为通用查询参数

        [MapperIgnoreSource(nameof(FileQuery.Page))]
        [MapperIgnoreSource(nameof(FileQuery.PageSize))]
        [MapperIgnoreSource(nameof(FileQuery.Order))]
        [MapperIgnoreSource(nameof(FileQuery.Scope))]
        [MapProperty(nameof(FileQuery.Title), nameof(FileSearchCriteria.Keyword))]
        public partial FileSearchCriteria ToCriteria(FileQuery query);

        [MapperIgnoreSource(nameof(CorpusQuery.Page))]
        [MapperIgnoreSource(nameof(CorpusQuery.PageSize))]
        [MapProperty(nameof(CorpusQuery.Content), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CorpusQuery query);


        [MapperIgnoreSource(nameof(FavoriteQuery.Page))]
        [MapperIgnoreSource(nameof(FavoriteQuery.PageSize))]
        [MapProperty(nameof(FavoriteQuery.CName), nameof(FavoriteSearchCriteria.Keyword))]
        public partial FavoriteSearchCriteria ToCriteria(FavoriteQuery query);

        [MapperIgnoreSource(nameof(CategoryQuery.Page))]
        [MapperIgnoreSource(nameof(CategoryQuery.PageSize))]
        [MapperIgnoreSource(nameof(CategoryQuery.SearchType))]
        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CategoryQuery query);
    }

    public class PageResultMapper<TEntity, TEntityDto>(IViewMapper<TEntity, TEntityDto> mapper)
        where TEntity : IEntity
        where TEntityDto : IEntityDto
    {
        public PageResult<TEntityDto> ToDto(PageResult<TEntity> source) => new(Convert(source.Items), source.Page, source.PageSize, source.HasMore);

        public List<TEntityDto> Convert(List<TEntity> source) => [.. source.Select(mapper.ToDto)];
    }
}
