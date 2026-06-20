using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared
{
    public interface IAQRequest { }
    public interface IAQRequest<out TResponse> :IAQRequest,IRequest<TResponse>{ }
    public interface IQuery<out TResponse> : IAQRequest<TResponse> { }

    public interface ICommand<out TResponse> : IAQRequest<TResponse> { }
    public interface ICreateCommand<out TResponse> : ICommand<TResponse> { }
    public interface IUpdateCommand<out TResponse> : ICommand<TResponse> { }
    public interface IDeleteCommand : ICommand<Unit> { }

    public interface IUploadRequest : IAQRequest
    {
        public IEnumerable<IFormFile> GetFiles();
    }


    public interface IMustCheckExistence<TEntity> where TEntity : IEntity
    {
        Guid UID { init; get; }
    }
}
