using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Exceptions;

public static class ExceptionExtensions
{
    public static IServiceCollection AddGlobalExceptionPolicy<THandler>(
        this IServiceCollection services)
        where THandler : class, IExceptionHandler
    {
        services.AddExceptionHandler<THandler>();
        return services.AddProblemDetails();
    }
}
