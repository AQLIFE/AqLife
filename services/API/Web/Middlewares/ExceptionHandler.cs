using AqLife.Shared;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Diagnostics;

namespace AqLife.Web.Middlewares
{

    public class BaseExceptionHandler(ILogger<IExceptionHandler> logger) : IExceptionHandler
    {
        protected virtual object GetResponseBody(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = GetHttpStatusCode(ex);
            return new { error = ex.Message };
        }
        private protected int GetHttpStatusCode(Exception ex)
        => ex is IApplicationException appEx ? (int)appEx.Behavior : (int)ExceptionBehavior.InternalError;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. 共有逻辑：统一日志记录 [cite: 61]
            logger.LogError(exception, @"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ApiType, $"发生未处理的异常：{exception.Message}");


            httpContext.Response.StatusCode = GetHttpStatusCode(exception);

            // 3. 差异化逻辑：由子类实现具体的响应体
            var response = GetResponseBody(httpContext, exception);
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }

    //public class ProductionExceptionHandler(ILogger<ProductionExceptionHandler> logger) : BaseExceptionHandler(logger)
    //{
    //    protected override object GetResponseBody(HttpContext context, Exception ex)
    //    {
    //        context.Response.StatusCode = GetHttpStatusCode(ex);
    //        return new { error = ex.Message };
    //    }
    //}

    //[Obsolete("意义不大且不利用开发")]
    //public class DevelopmentExceptionHandler(ILogger<DevelopmentExceptionHandler> logger) : BaseExceptionHandler(logger)
    //{
    //    protected override object GetResponseBody(HttpContext context, Exception ex)
    //    {
    //        var problemDetails = new ProblemDetails
    //        {
    //            Status = GetHttpStatusCode(ex),
    //            Title = "后端异常",
    //            Detail = $"系统内部发生错误：{ex.Message}\n{ex.GetType()}\n{ex.GetBaseException()}",
    //            Instance = context.Request.Path
    //        };
    //        return problemDetails;
    //    }        
    //}
}
