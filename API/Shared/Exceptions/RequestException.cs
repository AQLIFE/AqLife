using MyLife.Shared.Options;

namespace MyLife.Shared.Exceptions
{
    /// <summary>
    /// 业务基础异常,用于表示在API层发生的异常,例如请求参数无效,不合法
    /// </summary>
    /// <param name="message"></param>
    public class RequestException(string message) : BusinessException(message), IBusinessException
    {
        public override BehavioralLevel Level { get; set; } = BehavioralLevel.ApiType;
    }

    /// <summary>
    /// 请求在前置检查时失败的异常
    /// </summary>
    /// <param name="message"></param>
    public class RequestCheckException(string message) : BusinessException(message)
    {
        public override BehavioralLevel Level { get; set; } = BehavioralLevel.ValidType;
    }

    /// <summary>
    /// API 对应事务无法完成异常,用于表示在API层对应的事务无法完成,例如数据库操作失败、外部服务调用失败等情况
    /// </summary>
    public class RequestTransactionFailedException(string message) : RequestException(message) { }

    /// <summary>
    /// 请求登录 失败的异常
    /// </summary>
    /// <param name="message"></param>
    public class RequestBusinessLogicCheckException(string message) : RequestException(message) { }

    /// <summary>
    /// 无效的操作权限异常,用于表示在API层用户没有足够的权限执行某个操作
    /// </summary>
    /// <param name="message"></param>
    public class RequestAuthorizationException(string message) : RequestException(message) { }
}
