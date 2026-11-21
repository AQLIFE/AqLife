using AqLife.Application.Abstractions.Pipeline.Markers;
using Core.Search;
using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using Core.Pipeline.Behaviors;
using AqLife.Application.Business.Account;
using AqLife.Application.Business.Corpus;
using AqLife.Application.Business.File;
using AqLife.Application.Business.Tag;
using AqLife.Application.Mappers;
using AqLife.Application.Modules.Account;
using AqLife.Application.Modules.Account.Search;
using AqLife.Application.Modules.Account.Services;
using AqLife.Application.Modules.Category;
using AqLife.Application.Modules.Category.Search;
using AqLife.Application.Modules.Corpus;
using AqLife.Application.Modules.Corpus.Search;
using AqLife.Application.Modules.DevelopmentPlan;
using AqLife.Application.Modules.DevelopmentPlan.Search;
using AqLife.Application.Modules.DevelopmentPlan.Validator;
using AqLife.Application.Modules.Favorite;
using AqLife.Application.Modules.Favorite.Search;
using AqLife.Application.Modules.File;
using AqLife.Application.Modules.File.Search;
using AqLife.Application.Modules.File.Service;
using AqLife.Application.Modules.File.Validator;
using AqLife.Application.Modules.Tag;
using AqLife.Application.Modules.Tag.Search;
using AqLife.Application.Modules.Todo;
using AqLife.Application.Modules.Todo.Search;
using AqLife.Application.Modules.Todo.Validator;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Infrastructure.BackServices;
using AqLife.Shared.Contracts;
using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.Tools;
using Core.Mappers;
using Core.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UploadContext>();

        services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, DefaultAccount>(); 
        services.AddScoped<ISearchStrategy<CorpusEntity, EntitySearchCriteria>, AllCorpusSearchStrategy>();
        services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, AllFilesSearchStrategy>();
        services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, AllTagSearchStrategy>();
        services.AddScoped<ISearchStrategy<TodoEntity, TodoSearchCriteria>, AllTodoSearchStrategy>();
        services.AddScoped<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>, AllFavoriteSearchStrategy>();
        services.AddScoped<ISearchStrategy<CategoryEntity, EntitySearchCriteria>, AllCategorySearchStrategy>();
        services.AddScoped<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>, AllDevPlanSearchStrategy>();
        

        services.AddScoped<ISearchStrategy<AccountEntity, EntitySearchCriteria>, ValidAccount>();
        services.AddScoped<ISearchStrategy<CorpusEntity, EntitySearchCriteria>, FilteredCorpusSearchStrategy>();
        services.AddScoped<ISearchStrategy<FileMetaEntity, FileSearchCriteria>, FilteredFilesSearchStrategy>();
        services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, FilterTagSearchStrategy>();
        services.AddScoped<ISearchStrategy<TodoEntity, TodoSearchCriteria>, FilterTodoSearchStrategy>();
        services.AddScoped<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>, FilteredFavoriteSearchStrategy>();
        services.AddScoped<ISearchStrategy<CategoryEntity, EntitySearchCriteria>, FilteredCategorySearchStrategy>();
        services.AddScoped<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>, FilteredDevPlanSearchStrategy>();

        services.AddScoped<ISearch<AccountQuery, AccountDto, AccountEntity>, AccountSearch>();
        services.AddScoped<FileSearch>();
        services.AddScoped<ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity>>(sp => sp.GetRequiredService<FileSearch>());
        services.AddScoped<ISearch<CorpusQuery, PageResult<CorpusDto>, CorpusEntity>,CorpusSearch>();
        services.AddScoped<ISearch<TagQuery, PageResult<TagDto>, TagEntity>,TagSearch>();
        services.AddScoped<ISearch<TodoQuery, PageResult<TodoDto>, TodoEntity>,TodoSearch>();
        services.AddScoped<ISearch<FavoriteQuery, PageResult<FavoriteDto>, FavoriteItemEntity>,FavoriteSearch>();
        services.AddScoped<ISearch<CategoryQuery, PageResult<CategoryDto>, CategoryEntity>,CategorySearch>();
        services.AddScoped<ISearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, DevelopmentPlanEntity>,DevelopmentPlanSearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>>();
        services.AddScoped<ISearch<TodoRoadmapQuery, TodoRoadmapDto, DevelopmentPlanEntity>,DevelopmentPlanSearch<TodoRoadmapQuery, TodoRoadmapDto>>();

        services.AddScoped<FileReader>();
        services.AddScoped<FileWriter>();
        services.AddScoped<FileDeleter>();

        // File 模块对外只暴露资产契约；引用检查由各业务模块提供实现。
        services.AddScoped<AqLife.Application.Abstractions.Contracts.File.IFileAssetService, FileAssetService>();
        services.AddScoped<AqLife.Application.Abstractions.Contracts.File.IFileReferenceProvider, AqLife.Application.Modules.Account.Services.AccountFileReferenceProvider>();
        services.AddScoped<AqLife.Application.Abstractions.Contracts.File.IFileReferenceProvider, AqLife.Application.Modules.Favorite.FavoriteFileReferenceProvider>();

        services.AddSingleton<TodoMapper>();
        services.AddSingleton<IViewMapper<TodoEntity, TodoDto>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<IViewMapper<RoadmapNodeLayoutEntity, RoadmapNodeLayoutDto>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<IViewMapper<DevelopmentPlanEntity, TodoRoadmapDto>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<IViewMapper<TodoDependencyEntity, TodoDependencyDto>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<ICreateMapper<TodoEntity, CreateTodoCommand>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<IBaseEntityMapper<RoadmapNodeLayoutEntity, SaveRoadmapNodeLayoutCommand>>(sp => sp.GetRequiredService<TodoMapper>());
        services.AddSingleton<ISearchMapper<TodoQuery, PageResult<TodoDto>, TodoSearchCriteria>>(sp => sp.GetRequiredService<TodoMapper>());

        services.AddSingleton<DevelopmentPlanMapper>();
        services.AddSingleton<IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
        services.AddSingleton<ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
        services.AddSingleton<ISearchMapper<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());
        services.AddSingleton<ISearchMapper<TodoRoadmapQuery, TodoRoadmapDto, EntitySearchCriteria>>(sp => sp.GetRequiredService<DevelopmentPlanMapper>());

        services.AddSingleton<TagMapper>();
        services.AddSingleton<IViewMapper<TagEntity, TagDto>>(sp => sp.GetRequiredService<TagMapper>());
        services.AddSingleton<ICreateMapper<TagEntity, CreateTagCommand>>(sp => sp.GetRequiredService<TagMapper>());
        services.AddSingleton<ISearchMapper<TagQuery, PageResult<TagDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<TagMapper>());

        services.AddSingleton<SubscriptionMapper>();
        services.AddSingleton<IViewMapper<SubscriptionEntity, SubscriptionDto>>(sp => sp.GetRequiredService<SubscriptionMapper>());
        services.AddSingleton<ICreateMapper<SubscriptionEntity, ISubscription>>(sp => sp.GetRequiredService<SubscriptionMapper>());

        services.AddSingleton<AccountMapper>();
        services.AddSingleton<IViewMapper<AccountEntity, AccountDto>>(sp => sp.GetRequiredService<AccountMapper>());
        services.AddSingleton<ICreateMapper<AccountEntity, CreateAccountCommand>>(sp => sp.GetRequiredService<AccountMapper>());
        services.AddSingleton<ISearchMapper<AccountQuery, AccountDto, EntitySearchCriteria>>(sp => sp.GetRequiredService<AccountMapper>());

        services.AddSingleton<CorpusMapper>();
        services.AddSingleton<IViewMapper<CorpusEntity, CorpusDto>>(sp => sp.GetRequiredService<CorpusMapper>());
        services.AddSingleton<ICreateMapper<CorpusEntity, CreateCorpusCommand>>(sp => sp.GetRequiredService<CorpusMapper>());
        services.AddSingleton<ISearchMapper<CorpusQuery, PageResult<CorpusDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<CorpusMapper>());

        services.AddSingleton<FileMapper>();
        services.AddSingleton<IViewMapper<FileMetaEntity, FileDto>>(sp => sp.GetRequiredService<FileMapper>());
        services.AddSingleton<ICreateMapper<FileMetaEntity, CreateFileCommand>>(sp => sp.GetRequiredService<FileMapper>());
        services.AddSingleton<IBaseListEntityMapper<FileMetaEntity, UploadFilesCommand>>(sp => sp.GetRequiredService<FileMapper>());
        services.AddSingleton<ISearchMapper<FileQuery, PageResult<FileDto>, FileSearchCriteria>>(sp => sp.GetRequiredService<FileMapper>());

        services.AddSingleton<CategoryMapper>();
        services.AddSingleton<IViewMapper<CategoryEntity, CategoryDto>>(sp => sp.GetRequiredService<CategoryMapper>());
        services.AddSingleton<ICreateMapper<CategoryEntity, CreateCategoryCommand>> (sp => sp.GetRequiredService<CategoryMapper>());
        services.AddSingleton<ISearchMapper<CategoryQuery, PageResult<CategoryDto>, EntitySearchCriteria>>(sp => sp.GetRequiredService<CategoryMapper>());

        services.AddSingleton<FavoriteMediaMapper>();
        services.AddSingleton<FavoriteMapper>();
        services.AddSingleton<IViewMapper<FavoriteItemEntity, FavoriteDto>>(sp => sp.GetRequiredService<FavoriteMapper>());
        services.AddSingleton<ICreateMapper<FavoriteItemEntity, CreateFavoriteCommand>>(sp => sp.GetRequiredService<FavoriteMapper>());
        services.AddSingleton<ISearchMapper<FavoriteQuery, PageResult<FavoriteDto>, FavoriteSearchCriteria>>(sp => sp.GetRequiredService<FavoriteMapper>());


        //services.AddSingleton<QueryMapper>();
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
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UploadContextBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        var validatorTypes = implementationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition &&
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));
        services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(DevPlanValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(TodoUpdateValidator<>));
        services.AddScoped(typeof(IValidator<>), typeof(TodoAttributionValidityValidator<>));
        foreach (var type in validatorTypes)
            foreach (var item in type.GetInterfaces())
                services.AddScoped(item, type);

        var commandAssembly = typeof(IRequireValidEntity<>).Assembly;
        var commandTypes = commandAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequireValidEntity<>)));
        foreach (var commandType in commandTypes)
        {
            var entityType = commandType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequireValidEntity<>))
                .GetGenericArguments()[0];
            var validatorType = typeof(ExistenceValidator<,>).MakeGenericType(commandType, entityType);
            services.AddScoped(typeof(IValidator<>).MakeGenericType(commandType), validatorType);
        }

        return services;
    }
}