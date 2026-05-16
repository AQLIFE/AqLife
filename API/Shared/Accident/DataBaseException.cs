using MyLife.Shared.Options;

namespace MyLife.Shared.Accident
{
    public abstract class DataBaseException(string message) : BusinessException(message), IBusinessException
    {
        public new BehavioralLevel Level { get; set; } = BehavioralLevel.DbType;
    }

    /// <summary>
    /// 数据库连接异常,用于表示在数据库连接过程中发生的异常,例如数据库服务器不可达、连接字符串错误等情况
    /// </summary>
    /// <param name="message"></param>
    public class DataBaseConnectionException(string message) : DataBaseException(message)
    {
    }
}
