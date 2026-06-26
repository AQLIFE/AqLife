using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
