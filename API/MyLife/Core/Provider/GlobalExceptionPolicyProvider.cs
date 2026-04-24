using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyLife.Core.Provider
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

    public class ProductionExceptionHandler(ILogger<ProductionExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, @"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, $"发生未处理的异常：{exception.Message}");
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "服务器开小差了",
                Detail = "系统内部发生错误，请联系管理员或稍后再试。",
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }

    public class DevelopmentExceptionHandler(ILogger<DevelopmentExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, @"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, $"发生未处理的异常：{exception.Message}");
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "后端异常",
                Detail = $"系统内部发生错误：{exception.Message}",
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
