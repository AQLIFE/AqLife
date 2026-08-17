using Microsoft.AspNetCore.Http;
using MyLife.Domain.Contracts;

namespace MyLife.Domain.CommandInterface;

public interface IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles();
}

public interface IRequireTransaction { }

public interface IRequireValidEntity<TEntity> where TEntity : IEntity
{
    Guid UID { init; get; }
}
