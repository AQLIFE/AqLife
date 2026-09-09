using AqLife.Domain.Contracts;
using AqLife.Shared.IView;

namespace AqLife.Application.Abstractions.Mapper
{
    public interface ICreateMapper<T, TCreate> where T : IEntity
    {
        T ToEntity(TCreate createCommand);
    }

    // 2. 只负责更新的契约
    public interface IUpdateMapper<T, TUpdate> where T : IEntity
    {
        void UpdateEntity(TUpdate update, T source);
    }

    // 3. 只负责视图转换的契约
    public interface IViewMapper<T, TView> where T : IEntity where TView : IEntityDto
    {
        TView ToDto(T source);
    }
}
