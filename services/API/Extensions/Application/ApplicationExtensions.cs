using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.Contracts;
using Core.Mappers;
using Core.Pipeline.Behaviors;
using Core.Validators;
using AqLife.Application.Plan.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register each module's own services, mappers, search strategies and module-specific validators.
        AqLife.Application.Application.AddApplication(services);

        services.AddScoped(typeof(PageResultMapper<,>));

        var applicationAssemblies = AqLife.Application.Application.Assemblies.ToArray();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(applicationAssemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UploadContextBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            // Plan-specific rules run inside the transaction behavior.
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DependencyVersionIncrementBehavior<,>));
        });

        var applicationTypes = applicationAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .ToArray();

        var validatorTypes = applicationTypes
            .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition)
            .Where(type => type.GetInterfaces().Any(@interface =>
                @interface.IsGenericType &&
                @interface.GetGenericTypeDefinition() == typeof(IValidator<>)));

        foreach (var validatorType in validatorTypes)
        {
            foreach (var validatorInterface in validatorType.GetInterfaces()
                         .Where(@interface => @interface.IsGenericType &&
                             @interface.GetGenericTypeDefinition() == typeof(IValidator<>)))
            {
                services.AddScoped(validatorInterface, validatorType);
            }
        }

        // Commands live in their respective module assemblies, not in the marker-contract assembly.
        var commandTypes = applicationTypes
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.GetInterfaces().Any(@interface =>
                @interface.IsGenericType &&
                @interface.GetGenericTypeDefinition() == typeof(IRequireValidEntity<>)));

        foreach (var commandType in commandTypes)
        {
            var entityType = commandType.GetInterfaces()
                .First(@interface => @interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == typeof(IRequireValidEntity<>))
                .GetGenericArguments()[0];

            var validatorType = typeof(ExistenceValidator<,>).MakeGenericType(commandType, entityType);
            services.AddScoped(typeof(IValidator<>).MakeGenericType(commandType), validatorType);
        }

        return services;
    }
}