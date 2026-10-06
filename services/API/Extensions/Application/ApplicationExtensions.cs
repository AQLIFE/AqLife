using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.BackServices;
using AqLife.Application.Behaviors;
using AqLife.Application.Business;
using AqLife.Application.Business.Account;
using AqLife.Application.Business.Account.Search;
using AqLife.Application.Business.Account.Services;
using AqLife.Application.Business.Corpus;
using AqLife.Application.Business.Corpus.Search;
using AqLife.Application.Business.File;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Application.Business.File.Validator;
using AqLife.Application.Business.Tag;
using AqLife.Application.Business.Tag.Search;
using AqLife.Application.Business.Todo;
using AqLife.Application.Business.Todo.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<FileSecurityAspect>();
        services.AddScoped<UploadContext>();

        services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, AllFilesSearchStrategy>();
        services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, AllTagSearchStrategy>();
        services.AddScoped<ISearchStrategy<TodoEntity, EntitySearchCriteria>, AllTodoSearchStrategy>();
        services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, DefaultAccount>();
        services.AddScoped<ISearchStrategy<CorpusEntity, EntitySearchCriteria>, AllCorpusSearchStrategy>();

        services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, ValidAccount>();
        services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, FilteredFilesSearchStrategy>();
        services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, FilterTagSearchStrategy>();
        services.AddScoped<ISearchStrategy<TodoEntity, EntitySearchCriteria>, FilterTodoSearchStrategy>();

        services.AddScoped<AccountSearch>();
        services.AddScoped<FileSearch>();
        services.AddScoped<CorpusSearch>();
        services.AddScoped<TagSearch>();
        services.AddScoped<TodoSearch>();

        services.AddScoped<FileReader>();
        services.AddScoped<FileWriter>();
        services.AddScoped<FileDeleter>();

        services.AddSingleton<TodoMapper>();
        services.AddSingleton<IViewMapper<TodoEntity, TodoDto>>(sp =>
            sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<ICreateMapper<TodoEntity, CreateTodoCommand>>(sp =>
            sp.GetRequiredService<TodoMapper>());

        services.AddSingleton<TagMapper>();
        services.AddSingleton<IViewMapper<TagEntity, TagDto>>(sp =>
            sp.GetRequiredService<TagMapper>());
        services.AddSingleton<ICreateMapper<TagEntity, CreateTagCommand>>(sp =>
            sp.GetRequiredService<TagMapper>());

        services.AddSingleton<SubscriptionMapper>();
        services.AddSingleton<IViewMapper<SubscriptionEntity, SubscriptionDto>>(sp =>
            sp.GetRequiredService<SubscriptionMapper>());
        services.AddSingleton<ICreateMapper<SubscriptionEntity, ISubscription>>(sp =>
            sp.GetRequiredService<SubscriptionMapper>());

        services.AddSingleton<AccountMapper>();
        services.AddSingleton<IViewMapper<AccountEntity, AccountDto>>(sp =>
            sp.GetRequiredService<AccountMapper>());
        services.AddSingleton<ICreateMapper<AccountEntity, CreateAccountCommand>>(sp =>
            sp.GetRequiredService<AccountMapper>());

        services.AddSingleton<CorpusMapper>();
        services.AddSingleton<IViewMapper<CorpusEntity, CorpusDto>>(sp =>
            sp.GetRequiredService<CorpusMapper>());
        services.AddSingleton<ICreateMapper<CorpusEntity, CreateCorpusCommand>>(sp =>
            sp.GetRequiredService<CorpusMapper>());

        services.AddSingleton<FileMapper>();
        services.AddScoped<FileMappingService>();
        services.AddScoped<IViewMapper<FileMetaEntity, FileDto>>(sp =>
            sp.GetRequiredService<FileMappingService>());
        services.AddScoped<ICreateMapper<FileMetaEntity, IFormFile>>(sp =>
            sp.GetRequiredService<FileMappingService>());

        services.AddSingleton<QueryMapper>();
        services.AddScoped(typeof(PageResultMapper<,>));


        services.AddScoped<SystemInitializationService>();
        services.AddScoped<IFilePublishService, FilePublishService>();
        services.AddScoped<BlogSearch>();
        services.AddHostedService<ScheduledPublishWorker>();

        var implementationAssembly = typeof(AqLife.Application.Application).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(implementationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        var validatorTypes = implementationAssembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                !t.IsGenericTypeDefinition &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IValidator<>)));

        services.AddScoped(typeof(IValidator<>), typeof(ExistenceValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));

        foreach (var type in validatorTypes)
        {
            foreach (var item in type.GetInterfaces())
            {
                services.AddScoped(item, type);
            }
        }

        return services;
    }
}
