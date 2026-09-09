using AqLife.Application.Abstractions.Search;
using AqLife.Application.Behaviors;
using AqLife.Application.Business.Account.Search;
using AqLife.Application.Business.Corpus.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Application.Business.File.Validator;
using AqLife.Application.Business.Tag.Search;
using AqLife.Application.Business.Todo.Search;
using AqLife.Application.Search;
using AqLife.Application.Validators;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using AqLife.Application.Business;
using AqLife.Application.Business.Tag;
using AqLife.Application.Business.File;
using AqLife.Application.Business.Account;
using AqLife.Application.Mappers;
using AqLife.Application.Business.Corpus;
using AqLife.Application.Business.Todo;

namespace AqLife.Application
{
    public static class Application
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            var implementationAssembly = typeof(Application).Assembly;
            // 1. 扫描 MediatR (一次性扫描所有 Handler) [cite: 1, 191]
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(implementationAssembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            });


            // 2. 修复验证器注册：增加 !t.IsAbstract 过滤条件
            var validatorTypes = implementationAssembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition && t.GetInterfaces().Any(i => i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IValidator<>)));
            // 泛型验证器需要手动注册，因为它们是 open generic types，不能通过扫描程序集自动注册
            services.AddScoped(typeof(IValidator<>), typeof(ExistenceValidator<>)); // 业务ID的统一检查
            services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));
            foreach (var type in validatorTypes)
            {
                foreach (var item in type.GetInterfaces())
                {
                    // 💡 这种循环注册方式支持同一个接口有多个实现                    
                    services.AddScoped(item, type);// 这样 ValidationBehavior 就可以通过 IEnumerable<IValidator<T>> 获取到所有的验证规则
                }
            }

            // 3. 注册核心业务 Service [cite: 197, 198]
            services.AddScoped(typeof(ISearchStrategy<,>), typeof(AllSearchStrategyBase<,>));// 被继承
            //services.AddScoped(typeof(ISearchStrategy<,>), typeof(FilteredSearchStrategyBase<,>));// 被继承

            services.AddScoped<FileSecurityAspect>();// FileSearch 依赖
            services.AddScoped<PreviewContext>();
            services.AddScoped<UploadContext>();// UploadContext 提供给 FileService
            services.AddScoped<ISearchStrategy<FileMetaEntity, EntitySearchCriteria>, FilteredFilesSearchStrategy>();// FileSearch 专属策略
            services.AddScoped<ISearchStrategy<TagEntity, EntitySearchCriteria>, FilterTagSearchStrategy>();// Tag的策略
            services.AddScoped<ISearchStrategy<TodoEntity, EntitySearchCriteria>, FilterTodoSearchStrategy>();// Tag的策略

            // 注册所有 Query 业务类
            services.AddScoped<AccountSearch>();
            services.AddScoped<FileSearch>();
            services.AddScoped<CorpusSearch>();
            services.AddScoped<TagSearch>();
            services.AddScoped<TodoSearch>();

            // 注册具体Command 实际业务类
            //services.AddScoped<IFileStorage, LocalFileStorage>();
            services.AddScoped<FileReader>();
            services.AddScoped<FileWriter>();
            services.AddScoped<FileDeleter>();
            //services.AddScoped<FileService>();

            // 4. 注册所有 Mapper [cite: 198]
            services.AddSingleton<TodoMapper>();
            services.AddSingleton<TagMapper>();
            services.AddSingleton<FileMapper>();
            services.AddSingleton<SubscriptionMapper>();
            services.AddSingleton<AccountMapper>();
            services.AddSingleton<QueryMapper>();
            services.AddSingleton<CorpusMapper>();


            return services;
        }
    }
}
