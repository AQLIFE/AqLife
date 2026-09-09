using AqLife.Domain.Contracts;
using Microsoft.AspNetCore.Http;

namespace AqLife.Domain.CommandInterface;

public interface IHasFormFiles
{
    public IEnumerable<IFormFile> GetFiles();
}

public interface IRequireTransaction { }

public interface IRequireValidEntity<TEntity> where TEntity : IEntity
{
    Guid UID { init; get; }
}
