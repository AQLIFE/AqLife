using MyLife.Shared.Options;

namespace MyLife.Shared.Exceptions
{
    public abstract class DatabaseException(string message) : Exception(message), IApplicationException
    {
        public ExceptionBehavior Behavior { get; } = ExceptionBehavior.InternalError;
        public ExceptionCategory Category { get; } = ExceptionCategory.Database;
        public abstract string Code { get; }
    }

    /// <summary>
    /// 数据库连接异常,用于表示在数据库连接过程中发生的异常,例如数据库服务器不可达、连接字符串错误等情况
    /// </summary>
    /// <param name="message"></param>
    public class DataBaseConnectionException(string message) : DatabaseException(message)
    {
        public override string Code => "DB_CONN_ERR";
    }
}
