using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;
using System.Net;

namespace MyLife.Web.Middlewares
{

    public abstract class BaseExceptionHandler(ILogger<IExceptionHandler> logger) : IExceptionHandler
    {
        protected abstract object GetResponseBody(HttpContext context, Exception ex, int statusCode);

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. 共有逻辑：统一日志记录 [cite: 61]
            logger.LogError(exception, @"[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.ApiType, $"发生未处理的异常：{exception.Message}");

            // 2. 共有逻辑：统一状态码解析
            var statusCode = exception is BusinessException be ? be.Level switch
            {
                BehavioralLevel.ApiType or BehavioralLevel.ValidType => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            } : StatusCodes.Status500InternalServerError;

            httpContext.Response.StatusCode = statusCode;

            // 3. 差异化逻辑：由子类实现具体的响应体
            var response = GetResponseBody(httpContext, exception, statusCode);
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }

    public class ProductionExceptionHandler(ILogger<ProductionExceptionHandler> logger) : BaseExceptionHandler(logger)
    {
        protected override object GetResponseBody(HttpContext context, Exception ex, int statusCode)
        {
            if (ex is BusinessException businessException)
                context.Response.StatusCode = businessException.Level switch
                {
                    BehavioralLevel.ApiType or BehavioralLevel.ValidType => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
            else context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new { error = ex.Message };
        }
    }

    public class DevelopmentExceptionHandler(ILogger<DevelopmentExceptionHandler> logger) : BaseExceptionHandler(logger)
    {
        protected override object GetResponseBody(HttpContext context, Exception ex, int statusCode)
        {
            //logger.LogError(exception, @"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ApiType, $"发生未处理的异常：{exception.Message}");
            var problemDetails = new ProblemDetails
            {
                Status = ex is BusinessException businessException ? businessException.Level switch
                {
                    BehavioralLevel.ApiType or BehavioralLevel.ValidType => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                } : StatusCodes.Status500InternalServerError,
                Title = "后端异常",
                Detail = $"系统内部发生错误：{ex.Message}\n{ex.GetType()}\n{ex.GetBaseException()}",
                Instance = context.Request.Path
            };
            return problemDetails;
        }
    }
}
