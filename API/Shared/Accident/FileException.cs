using MyLife.Shared.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.Accident
{
    /// <summary>
    /// 文件类异常,用于表示与文件操作相关的异常,可以在此基础上派生出更具体的文件异常类型,以便于在全局异常处理中进行区分和处理
    /// </summary>
    /// <param name="message"></param>
    internal abstract  class FileException(string message) : BusinessException(message)
    {
    }

    /// <summary>
    /// 服务器本地或数据库未找到文件异常,当服务器无法找到指定的文件时抛出此异常,可以在全局异常处理中捕获并返回适当的错误信息给客户端,提示用户检查文件路径或文件是否存在
    /// </summary>
    /// <param name="message"></param>
    internal class NotFoundFileException(string message) : FileException(message)
    {
    }

    /// <summary>
    /// 文件类型不匹配异常,当上传的文件类型与预期不符时抛出此异常,可以在全局异常处理中捕获并返回适当的错误信息给客户端,提示用户上传正确的文件类型
    /// </summary>
    /// <param name="message"></param>
    internal class FileTypeMismatch(string message): FileException(message)
    {
    }


}
