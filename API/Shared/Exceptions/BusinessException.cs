using MyLife.Shared.Options;

namespace MyLife.Shared.Exceptions
{
    public interface IBusinessException
    {
        /// <summary>
        /// 错误代码,用于表示异常的类型或级别,可以在全局异常处理中根据错误代码进行不同的处理逻辑
        /// </summary>
        BehavioralLevel Level { get; set; }
    }

    /// <summary>
    /// 基础业务异常类,用于表示业务逻辑层发生的异常,可以在此基础上派生出更具体的业务异常类型,以便于在全局异常处理中进行区分和处理
    /// </summary>
    public abstract class BusinessException(string message) : Exception(message), IBusinessException
    {
        /// <summary>
        /// 内部错误等级
        /// </summary>
        public virtual BehavioralLevel Level { get; set; } = BehavioralLevel.ApiType;

    }
}
