namespace MyLife.Web.BusinessSecurity.RuntimeCheck
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
