using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Business.Favorite.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Contracts;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public partial class QueryMapper
    {
        [MapperIgnoreSource(nameof(TodoQuery.Page))]
        [MapperIgnoreSource(nameof(TodoQuery.PageSize))]
        [MapperIgnoreSource(nameof(TodoQuery.IsTree))]
        [MapperIgnoreSource(nameof(TodoQuery.PlanId))]
        [MapperIgnoreSource(nameof(TodoQuery.IncludeCompleted))]
        [MapperIgnoreSource(nameof(TodoQuery.IncludeArchived))]
        [MapperIgnoreSource(nameof(TodoQuery.ParentId))]
        [MapProperty(nameof(TodoQuery.Desc), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TodoQuery query);


        [MapperIgnoreSource(nameof(TagQuery.Page))]
        [MapperIgnoreSource(nameof(TagQuery.PageSize))]
        [MapProperty(nameof(TagQuery.Tag), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TagQuery query);



        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(AccountQuery query);

        [MapperIgnoreSource(nameof(FileQuery.Page))]
        [MapperIgnoreSource(nameof(FileQuery.PageSize))]
        [MapperIgnoreSource(nameof(FileQuery.Order))]
        [MapperIgnoreSource(nameof(FileQuery.Scope))]
        [MapProperty(nameof(FileQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial FileSearchCriteria ToCriteria(FileQuery query);

        [MapperIgnoreSource(nameof(CorpusQuery.Page))]
        [MapperIgnoreSource(nameof(CorpusQuery.PageSize))]
        [MapProperty(nameof(CorpusQuery.Content), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CorpusQuery query);


        [MapperIgnoreSource(nameof(FavoriteQuery.Page))]
        [MapperIgnoreSource(nameof(FavoriteQuery.PageSize))]
        [MapProperty(nameof(FavoriteQuery.CName), nameof(FavoriteSearchCriteria.Keyword))]
        public partial FavoriteSearchCriteria ToCriteria(FavoriteQuery query);
    }

    public partial class PageResultMapper<TEntity, TEntityDto>(IViewMapper<TEntity, TEntityDto> mapper)
        where TEntity : IEntity
        where TEntityDto : IEntityDto
    {
        public PageResult<TEntityDto> ToDto(PageResult<TEntity> source) => new(Convert(source.Items), source.Page, source.PageSize, source.HasMore);

        public List<TEntityDto> Convert(List<TEntity> source) => [.. source.Select(mapper.ToDto)];
    }
}
