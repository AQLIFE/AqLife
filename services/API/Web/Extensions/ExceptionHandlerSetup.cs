using AqLife.Web.Middlewares;

namespace AqLife.Web.Extensions
{

    public static class ExceptionHandlerSetup
    {
        public static IServiceCollection AddGlobalExceptionPolicy(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddExceptionHandler<BaseExceptionHandler>();
            return services.AddProblemDetails();
        }
    }
}
