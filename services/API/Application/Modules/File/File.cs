using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.File.Search;
using AqLife.Application.File.Service;
using AqLife.Application.File.Validator;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Contracts;
using AqLife.Shared.Tools;
using Core.Search;
using File;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Application.File
{
    public static class File
    {
        public static IServiceCollection AddFile(this IServiceCollection services)
        {
            services.AddScoped<UploadContext>();

            services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, AllFilesSearchStrategy>();
            services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, FilteredFilesSearchStrategy>();
            services.AddScoped<FileSearch>();
            services.AddScoped<ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity>>(sp => sp.GetRequiredService<FileSearch>());

            services.AddScoped<FileReader>();
            services.AddScoped<FileWriter>();
            services.AddScoped<FileDeleter>();

            services.AddScoped<IFileAssetService, FileAssetService>();

            services.AddSingleton<FileMapper>();
            services.AddSingleton<IViewMapper<FileMetaEntity, FileDto>>(sp => sp.GetRequiredService<FileMapper>());
            services.AddSingleton<ICreateMapper<FileMetaEntity, CreateFileCommand>>(sp => sp.GetRequiredService<FileMapper>());
            services.AddSingleton<IBaseListEntityMapper<FileMetaEntity, UploadFilesCommand>>(sp => sp.GetRequiredService<FileMapper>());
            services.AddSingleton<ISearchMapper<FileQuery, PageResult<FileDto>, FileSearchCriteria>>(sp => sp.GetRequiredService<FileMapper>());

            services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));
            services.AddScoped<IScheduledPublishProcessor, ScheduledPublishProcessor>();
            services.AddScoped<ResumableFileUploadService>();

            return services;
        }
    }
}
