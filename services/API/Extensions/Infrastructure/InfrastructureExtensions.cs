using Amazon.Runtime;
using Amazon.S3;
using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Infrastructure.Authentication;
using AqLife.Infrastructure.FileStorage;
using AqLife.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AqLife.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext, AppStorage>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<R2Options>>()
                .Value;

            var credentials = new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey);

            return new AmazonS3Client(
                credentials,
                new AmazonS3Config
                {
                    ServiceURL = options.Endpoint,
                    ForcePathStyle = true
                });
        });

        services.AddScoped<IFileStorage, R2FileStorage>();
        services.AddScoped<ITokenProvider<AccountEntity>, TokenProvider>();

        Log.Debug(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "当前使用默认存储方案: Cloudflare R2");

        return services;
    }
}
