using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Behaviors
{

    public class UploadContextBehavior<TRequest, TResponse>(UploadContext uploadContext): IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IAppRequest
    {
        public async Task<TResponse> Handle(TRequest request,RequestHandlerDelegate<TResponse> next,CancellationToken cancellationToken)
        {
            if (request is not IHasFormFiles fileRequest)
                return await next(cancellationToken);

            var files = fileRequest.GetFiles();

            if (files is null || !files.Any())
                throw new RequestCheckException("上传文件不能为空");

            foreach (var file in files)
            {
                var hash = await CalculateHashAsync(file, cancellationToken);
                uploadContext.FileHashes.Add(file, hash);
            }

            return await next(cancellationToken);
        }

        private static async Task<string> CalculateHashAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();

            using var sha256 = System.Security.Cryptography.SHA256.Create();

            var hashBytes = await sha256.ComputeHashAsync(
                stream,
                cancellationToken);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
