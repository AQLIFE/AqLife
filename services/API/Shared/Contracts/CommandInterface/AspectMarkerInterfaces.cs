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
