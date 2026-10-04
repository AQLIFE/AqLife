using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Exception;

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
