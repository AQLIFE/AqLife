using AqLife.Shared.Contracts;
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
