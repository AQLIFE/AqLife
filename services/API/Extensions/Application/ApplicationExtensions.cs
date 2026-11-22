using AqLife.Application.Abstractions.Pipeline.Markers;
using Core.Pipeline.Behaviors;

using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
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
        services.AddScoped(typeof(PageResultMapper<,>));

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