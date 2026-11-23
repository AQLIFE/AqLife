using AqLife.Application.Abstractions.Contracts.File;
using Amazon.Runtime;
using Amazon.S3;
using AqLife.Application.Abstractions.Authentication;
using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.File.Service;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Persistence.Queries;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities;
using AqLife.Infrastructure.Authentication;
using AqLife.Infrastructure.FileStorage;
using AqLife.Infrastructure.BackServices;
using AqLife.Infrastructure.Persistence;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace AqLife.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped(typeof(IEntityExistenceChecker<>), typeof(EfEntityExistenceChecker<>));
        services.AddScoped<IAccountQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<IAccountPersistence>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ICategoryQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ICategoryPersistence>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ICorpusQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ICorpusPersistence>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<IFavoriteItemQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<IFileMetaQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<IDevelopmentPlanQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ITodoQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<ITagQuerySource>(sp => sp.GetRequiredService<AppStorage>());
        services.AddScoped<IResumableFileUploadService, ResumableFileUploadService>();
        services.AddScoped<IScheduledFileQuery, ScheduledFileQuery>();
        services.AddScoped<IFilePublishService, FilePublishService>();
        services.AddHostedService<ScheduledPublishWorker>();

        if (!environment.IsTest()) // 非 Test 模式
        {
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
        }
        else services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<ITokenProvider<AccountEntity>, TokenProvider>();

        Log.Debug(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "当前使用默认存储方案: Cloudflare R2");

        return services;
    }
    public static bool IsTest(this IHostEnvironment hostEnvironment)
    => hostEnvironment.IsEnvironment("Test");
}
