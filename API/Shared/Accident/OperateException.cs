using MyLife.Shared.Options;

namespace MyLife.Shared.Accident
{
    /// <summary>
    /// controller 层基础异常,用于表示在API层发生的异常,例如请求参数错误、权限不足等情况
    /// </summary>
    /// <param name="message"></param>
    public abstract class OperateException(string message) : BusinessException(message), IBusinessException
    {
        public override BehavioralLevel Level { get; set; } = BehavioralLevel.ApiType;
    }

    /// <summary>
    ///  Controller 层接收到的请求参数错误异常,用于表示在API层接收到的请求参数错误,例如缺少必需的参数、参数类型不正确等情况
    /// </summary>
    /// <param name="message"></param>
    public class OperateRequestException(string message) : OperateException(message) { }

    /// <summary>
    /// The transaction could not be completed.
    /// API 对应事务无法完成异常,用于表示在API层对应的事务无法完成,例如数据库操作失败、外部服务调用失败等情况
    /// </summary>
    public class OperateTransactionFailedException(string message) : OperateException(message) { }


    public class OperateBusinessLogicCheckException(string message) : OperateException(message) { }

    /// <summary>
    /// 无效的操作权限异常,用于表示在API层用户没有足够的权限执行某个操作
    /// </summary>
    /// <param name="message"></param>
    public class OperateAuthorizationException(string message) : OperateException(message) { }
}
