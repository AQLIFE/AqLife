using AqLife.Shared.Options;

namespace AqLife.Shared.Exceptions
{
    /// <summary>
    /// 基础业务异常类,用于表示业务逻辑层发生的异常,可以在此基础上派生出更具体的业务异常类型,以便于在全局异常处理中进行区分和处理
    /// </summary>
    public abstract class BusinessException(string message) : Exception(message), IApplicationException
    {
        public virtual ExceptionCategory Category
            => ExceptionCategory.Operation;
        public virtual ExceptionBehavior Behavior
            => ExceptionBehavior.BadRequest;
        public abstract string Code { get; }
    }
    /// <summary>
    /// 请求失败
    /// </summary>
    /// <param name="message"></param>
    public class RequestFailException(string message) : BusinessException(message)
    {
        public override string Code { get; } = "RequestFail";
    }

    /// <summary>
    /// 请求在前置检查时失败的异常
    /// </summary>
    /// <param name="message"></param>
    public class RequestCheckException(string message) : BusinessException(message)
    {
        public override ExceptionBehavior Behavior => ExceptionBehavior.BadRequest;// 400
        public override string Code { get; } = "RequestCheckFailed";
    }
    /// <summary>
    ///  身份鉴权相关的异常表达
    /// </summary>
    /// <param name="message"></param>
    public class AuthenticationException(string message) : RequestCheckException(message)
    {
        public override ExceptionBehavior Behavior => ExceptionBehavior.Unauthorized;// 401
    }
    /// <summary>
    /// 无效的操作权限异常,用于表示在API层用户没有足够的权限执行某个操作
    /// </summary>
    /// <param name="message"></param>
    public class ForbiddenException(string message) : BusinessException(message)
    {
        public override ExceptionBehavior Behavior => ExceptionBehavior.Forbidden;// 403
        public override string Code { get; } = "Forbidden";
    }
    /// <summary>
    /// 资源 找不到或丢失
    /// </summary>
    /// <param name="message"></param>
    public class ResourceNotFoundException(string message) : BusinessException(message)
    {
        public override ExceptionBehavior Behavior => ExceptionBehavior.NotFound;//404
        public override string Code { get; } = "ResourceNotFound";
    }
    public class ResourceConflictException(string message) : BusinessException(message)
    {
        public override ExceptionBehavior Behavior => ExceptionBehavior.Conflict;//409
        public override string Code { get; } = "ResourceConflict";
    }

    /// <summary>
    ///  数据模型合法性异常
    /// </summary>
    public class DomainLegalityException(string message) : Exception(message), IApplicationException
    {
        public ExceptionCategory Category => ExceptionCategory.Resource;
        public ExceptionBehavior Behavior => ExceptionBehavior.BadRequest;
        public string Code { get; } = "ResourceIllegal";
    }
}
