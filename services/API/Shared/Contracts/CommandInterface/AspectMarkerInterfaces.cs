using Microsoft.AspNetCore.Http;

namespace AqLife.Shared.Contracts.CommandInterface;
/// <summary>
/// 标识接口, 标识启用UploadContext
/// </summary>
public interface IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles();
}
/// <summary>
/// 标识接口,自动开启事务管理
/// </summary>
public interface IRequireTransaction { }

/// <summary>
/// 标识接口, 自动检查对应Entity 的UID是否存在
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRequireValidEntity<out TEntity> where TEntity : IEntity
{
    Guid UID { init; get; }
}

/// <summary>
/// 标识接口 , 自动触发 开发计划存档 规则检查 : 存档即不允许任何 ICommand
/// </summary>
public interface IRequireDevPlan{
    public Guid PlanId { get; init; }
}
/// <summary>
/// 标识接口 , 在事务中会自动更新依赖图版本;同时该接口的集成还包含了 IRequireDevPlan 的检查业务
/// </summary>
public interface IRequireDependencyVersionIncrement : IRequireDevPlan, IRequireTransaction { }

/// <summary>
/// 标识接口, 用于在 Search 类中做分支查询处理
/// </summary>
public interface IReadDevPlan { }