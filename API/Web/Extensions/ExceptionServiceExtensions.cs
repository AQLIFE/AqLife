using MyLife.Web.Middlewares;

namespace MyLife.Web.Extensions
{

    public static class ExceptionServiceExtensions
    {
        public static IServiceCollection AddGlobalExceptionPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            if (environment.IsProduction())
                services.AddExceptionHandler<ProductionExceptionHandler>();
            else
                services.AddExceptionHandler<DevelopmentExceptionHandler>();
            return services.AddProblemDetails();
        }
    }
}
