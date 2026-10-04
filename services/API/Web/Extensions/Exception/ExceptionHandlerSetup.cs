using AqLife.Web.Middlewares;

namespace AqLife.Web.Extensions.Exception
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
