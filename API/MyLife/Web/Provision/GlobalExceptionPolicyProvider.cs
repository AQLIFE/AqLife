using MyLife.Web.Middlewares;

namespace MyLife.Web.Provision
{

    public static class GlobalExceptionPolicyProvider
    {
        public static IServiceCollection BindGlobalExceptionPolicy(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsProduction())
                builder.Services.AddExceptionHandler<ProductionExceptionHandler>();
            else
                builder.Services.AddExceptionHandler<DevelopmentExceptionHandler>();
            return builder.Services.AddProblemDetails();
        }
    }
}
