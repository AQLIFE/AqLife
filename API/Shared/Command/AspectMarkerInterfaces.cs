using Microsoft.AspNetCore.Http;
using MyLife.Shared.Contracts;

namespace MyLife.Shared.Command;

public interface IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles();
}

public interface IRequireTransaction { }

public interface IRequireValidEntity<TEntity> where TEntity : IEntity
{
    Guid UID { init; get; }
}
