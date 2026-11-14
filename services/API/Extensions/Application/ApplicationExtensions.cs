using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.BackServices;
using AqLife.Application.Behaviors;
using AqLife.Application.Business.Account;
using AqLife.Application.Business.Account.Search;
using AqLife.Application.Business.Account.Services;
using AqLife.Application.Business.Category.Search;
using AqLife.Application.Business.Corpus;
using AqLife.Application.Business.Corpus.Search;
using AqLife.Application.Business.DevelopmentPlan.Search;
using AqLife.Application.Business.Favorite.Search;
using AqLife.Application.Business.File;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Application.Business.File.Validator;
using AqLife.Application.Business.Tag;
using AqLife.Application.Business.Tag.Search;
using AqLife.Application.Business.Todo.Search;
using AqLife.Application.Business.Todo.Validator;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Contracts;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
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

        services.AddScoped<ISearch<AccountQuery, AccountEntity>, AccountSearch>();
        services.AddScoped<FileSearch>();
        services.AddScoped<ISearch<FileQuery, FileMetaEntity>>(sp => sp.GetRequiredService<FileSearch>());
        services.AddScoped<ISearch<CorpusQuery, CorpusEntity>,CorpusSearch>();
        services.AddScoped<ISearch<TagQuery, TagEntity>,TagSearch>();
        services.AddScoped<ISearch<TodoQuery, TodoEntity>,TodoSearch>();
        services.AddScoped<ISearch<FavoriteQuery, FavoriteItemEntity>,FavoriteSearch>();
        services.AddScoped<ISearch<CategoryQuery, CategoryEntity>,CategorySearch>();
        services.AddScoped<ISearch<DevelopmentPlanQuery, DevelopmentPlanEntity>,DevelopmentPlanSearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>>();
        services.AddScoped<ISearch<TodoRoadmapQuery, DevelopmentPlanEntity>,DevelopmentPlanSearch<TodoRoadmapQuery, TodoRoadmapDto>>();

        services.AddScoped<FileReader>();
        services.AddScoped<FileWriter>();
        services.AddScoped<FileDeleter>();

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