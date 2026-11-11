using Microsoft.AspNetCore.Http;

namespace AqLife.Shared.Contracts.CommandInterface;

public interface IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles();
}

public interface IRequireTransaction { }

public interface IRequireValidEntity<out TEntity> where TEntity : IEntity
{
    Guid UID { init; get; }
}

/// <summary>
/// 标识接口 , 集成该接口的类,应有专用的验证器处理
/// </summary>
public interface IRequireDevPlan{
    public Guid PlanId { get; init; }
}
/// <summary>
/// 标识接口 , 集成该接口的类,在事务中的主业务完成后 会自动更新 依赖图版本;同时该接口的集成还包含了 IRequireDevPlan 的检查业务
/// </summary>
public interface IRequireDependencyVersionIncrement : IRequireDevPlan, IRequireTransaction { }

/// <summary>
/// 标识接口, 集成该接口的类,用于在 Search 类中做泛型处理
/// </summary>
public interface IReadDevPlan { }