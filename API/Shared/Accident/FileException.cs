namespace MyLife.Shared.Accident
{
    /// <summary>
    /// 文件类异常,用于表示与文件操作相关的异常,可以在此基础上派生出更具体的文件异常类型,以便于在全局异常处理中进行区分和处理
    /// </summary>
    /// <param name="message"></param>
    public abstract class FileException(string message) : BusinessException(message)
    {
    }


    /// <summary>
    /// 文件类型不匹配异常,当上传的文件类型与预期不符时抛出此异常,可以在全局异常处理中捕获并返回适当的错误信息给客户端,提示用户上传正确的文件类型
    /// </summary>
    /// <param name="message"></param>
    public class FileTypeMismatch(string message) : FileException(message) { }
}
