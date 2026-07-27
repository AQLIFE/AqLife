using MyLife.Web.Middlewares;

namespace MyLife.Web.Extensions
{

    public static class ExceptionHandlerSetup
    {
        public static IServiceCollection AddGlobalExceptionPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            //if (environment.IsProduction())
            //    services.AddExceptionHandler<ProductionExceptionHandler>();
            //else
            //    services.AddExceptionHandler<DevelopmentExceptionHandler>();
            services.AddExceptionHandler<BaseExceptionHandler>();
            return services.AddProblemDetails();
        }
    }
}
