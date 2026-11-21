using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;

namespace Core.Pipeline.Behaviors
{
    public class UploadContextBehavior<TRequest, TResponse>(UploadContext uploadContext) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IApplicationRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not IHasFormFiles fileRequest)
                return await next(cancellationToken);

            var files = fileRequest.GetFiles();

            if (files is null || !files.Any())
                throw new RequestCheckException("上传文件不能为空");

            foreach (var file in files)
                await uploadContext.GetOrAddFileHashAsync(file, cancellationToken);

            return await next(cancellationToken);
        }
    }
}
