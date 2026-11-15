using AqLife.Application.Abstractions.Mapper;
using AqLife.Shared.Contracts;
using AqLife.Shared.IView;

namespace AqLife.Application.Mappers
{
    public class PageResultMapper<TEntity, TEntityDto>(IViewMapper<TEntity, TEntityDto> mapper)
        where TEntity : IEntity
        where TEntityDto : IEntityDto
    {
        public PageResult<TEntityDto> ToDto(PageResult<TEntity> source) => new(Convert(source.Items), source.Page, source.PageSize, source.HasMore);

        public List<TEntityDto> Convert(List<TEntity> source) => [.. source.Select(mapper.ToDto)];
    }
}
