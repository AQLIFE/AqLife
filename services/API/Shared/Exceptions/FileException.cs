using MyLife.Shared.Options;

namespace MyLife.Shared.Exceptions
{
    /// <summary>
    /// 文件类异常,用于表示与文件操作相关的异常,可以在此基础上派生出更具体的文件异常类型,以便于在全局异常处理中进行区分和处理
    /// </summary>
    /// <param name="message"></param>
    public abstract class FileException(string message) : Exception(message), IApplicationException
    {
        public virtual ExceptionCategory Category { get; } = ExceptionCategory.Resource;
        public virtual ExceptionBehavior Behavior { get; } = ExceptionBehavior.BadRequest;
        public abstract string Code { get; }
    }

    //public class FileTypeMismatchException(string message) : FileException(message) 
    //{
    //    public override ExceptionCategory Category { get; } = ExceptionCategory.Validation;
    //}
    //public class FileSizeNotSupportedException(string message) : FileException(message) {
    //    public override ExceptionCategory Category { get; } = ExceptionCategory.Validation;
    //}
    /// <summary>
    ///  文件业务 针对文件的校验异常
    /// </summary>
    /// <param name="message"></param>
    public class FileValidationException(string message) : FileException(message)
    {
        public override ExceptionCategory Category { get; } = ExceptionCategory.Validation;
        public override string Code { get; } = "FileValidationError";
    }

    public class FileParseException(string message) : FileException(message)
    {
        public override string Code { get; } = "FileParseError";
    }
}
