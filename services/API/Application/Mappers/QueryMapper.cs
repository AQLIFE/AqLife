using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
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
        [MapProperty(nameof(FileQuery.Title), nameof(EntitySearchCriteria.Keyword))]
        public partial FileSearchCriteria ToCriteria(FileQuery query);

        [MapperIgnoreSource(nameof(CorpusQuery.Page))]
        [MapperIgnoreSource(nameof(CorpusQuery.PageSize))]
        [MapProperty(nameof(CorpusQuery.Content), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(CorpusQuery query);
    }

    public class PageResultMapper<TEntity, TEntityDto>(IViewMapper<TEntity, TEntityDto> mapper)
        where TEntity :IEntity
        where TEntityDto : IEntityDto
    {
        public PageResult<TEntityDto> ToDto(PageResult<TEntity> source)
        {
            List<TEntityDto> dtos = source.Items.Select<TEntity,TEntityDto>(mapper.ToDto).ToList();
            return new PageResult<TEntityDto>(dtos, source.Page, source.PageSize, source.HasMore);
        }
    }
}
