using MyLife.Shared.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.Accident
{
    /// <summary>
    /// controller 层基础异常,用于表示在API层发生的异常,例如请求参数错误、权限不足等情况,可以在全局异常处理中捕获并进行相应的处理,如记录日志、提示用户等
    /// </summary>
    /// <param name="message"></param>
    internal abstract class OperateException(string message):BusinessException(message),IBusinessException
    {
        public override BehavioralLevel Level { get; set; } = BehavioralLevel.ApiType;
    }

    /// <summary>
    ///  Controller 层接收到的请求参数错误异常,用于表示在API层接收到的请求参数错误,例如缺少必需的参数、参数类型不正确等情况,可以在全局异常处理中捕获并进行相应的处理,如记录日志、提示用户等
    /// </summary>
    /// <param name="message"></param>
    internal class APIRequestException(string message) : OperateException(message)
    {
    }

    /// <summary>
    /// The transaction could not be completed.
    /// API 对应事务无法完成异常,用于表示在API层对应的事务无法完成,例如数据库操作失败、外部服务调用失败等情况,可以在全局异常处理中捕获并进行相应的处理,如记录日志、提示用户等
    /// </summary>

    internal class  APITransactionFailedException(string message) : OperateException(message)
    {        
    }
}
