using AqLife.Application.Abstractions.Search;
using AqLife.Shared.Contracts;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AqLife.Application.Abstractions.Mapper
{
    public interface IBaseMapper<TSource,TTarget> { }

    public interface IBaseEntityMapper<TSource, TTarget>: IBaseMapper<TSource, TTarget> {
        public TSource ToEntity(TTarget target);
    }

    public interface IBaseListEntityMapper<TSource, TTarget> : IBaseMapper<TSource, TTarget>
    {
        public IEnumerable<TSource> ToEntity(TTarget target);
    }

    public interface    ICreateMapper<T, TCreate> : IBaseMapper<T,TCreate>
        where T : IEntity
        //where TCreate : ICreateCommand
    {
        T ToEntity(TCreate createCommand);
    }

    // 2. 只负责更新的契约
    public interface IUpdateMapper<T, TUpdate> : IBaseMapper<T, TUpdate>
        where T : IEntity
        where TUpdate : IUpdateCommand
    {
        void UpdateEntity(TUpdate update, T source);
    }

    // 3. 只负责视图转换的契约
    public interface IViewMapper<T, TView> : IBaseMapper<T, TView>
        where T : IEntity where TView : IEntityDto
    {
        TView ToDto(T source);
    }

    public interface ISearchMapper<TQuery,TResource, TCriteria> : IBaseMapper<TQuery, TCriteria>
        where TQuery : IQuery<TResource> where TCriteria : ISearchCriteria
        where TResource :class
    {
        TCriteria ToCriteria(TQuery query);
    }
    /// <summary>
    /// 专用于 具有联合主键关系表的转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TView"></typeparam>
    public interface IRelationshipViewMapper<T, TView> : IBaseMapper<T, TView>
        where T : IRelationship where TView : IEntityDto
    {
        TView ToDto(T source);
    }

    public interface IRelationshipUpdateMapper<T, TUpdate> : IBaseMapper<T, TUpdate>
        where T : IRelationship where TUpdate : IUpdateCommand
    {
        T ToEntity(TUpdate source);
    }


}
