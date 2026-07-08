using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Application.Behaviors;
using MyLife.Application.Validators;
using MyLife.Application.Validators.BusinessValidator;
using MyLife.Service.EntityService;
using MyLife.Service.Implementations;
using MyLife.Service.MapperService;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Service.ServiceInterfaces.IStrategy.Strategy.FileSearch;
using MyLife.Shared.Contracts;
using MyLife.Shared.Tools;

namespace MyLife.Application
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
            services.AddScoped(typeof(IValidator<>), typeof(ExistenceValidator<>));
            // 泛型验证器需要手动注册，因为它们是 open generic types，不能通过扫描程序集自动注册
            services.AddScoped(typeof(IValidator<>), typeof(FileTypeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileSizeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(FileDuplicateValidator<>));
            foreach (var type in validatorTypes)
            {
                foreach (var item in type.GetInterfaces())
                {
                    // 💡 这种循环注册方式支持同一个接口有多个实现
                    // 这样你的 ValidationBehavior 就可以通过 IEnumerable<IValidator<T>> 获取到所有的验证规则
                    services.AddScoped(item, type);
                }
            }

            // 3. 注册核心业务 Service [cite: 197, 198]
            services.AddScoped<UploadContext>();// UploadContext 提供给 FileService
            services.AddScoped<ISearchStrategy, FilteredFilesSearchStrategy>();//FilteredFilesSearchStrategy 提供给 FileSearch
            services.AddScoped<FileSearch>();
            services.AddScoped<FileService>();
            services.AddScoped<TagServices>();
            services.AddScoped<AccountService>();

            // 4. 注册所有 Mapper [cite: 198]
            services.AddSingleton<TodoMapper>();
            services.AddSingleton<TagMapper>();
            services.AddSingleton<FileMapper>();
            services.AddSingleton<SubscriptionMapper>();
            services.AddSingleton<AccountMapper>();


            return services;
        }


        private static Type? GetGenericBaseType(Type? currentType, Type targetGenericType)
        {
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == targetGenericType)
                {
                    return currentType;
                }
                currentType = currentType.BaseType;
            }
            return null;
        }
    }
}
