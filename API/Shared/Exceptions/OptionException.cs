using MyLife.Shared.Options;

namespace MyLife.Shared.Exceptions
{
    /// <summary>
    /// 应用程序配置异常,用于表示在应用程序自检配置过程中发生的异常,例如配置项缺失、配置值无效等情况,可以在全局异常处理中捕获并进行相应的处理,如记录日志、提示用户等
    /// </summary>
    /// <param name="message"></param>
    public abstract class ConfigurationException(string message) : Exception(message), IApplicationException
    {
        public virtual ExceptionCategory Category { get; } = ExceptionCategory.Configuration;
        public virtual ExceptionBehavior Behavior { get; } = ExceptionBehavior.InternalError;
        public abstract string Code { get; }
    }

    /// <summary>
    /// 配置项未找到异常,用于表示在应用程序自检配置过程中未找到指定的配置项
    /// 
    /// 例如配置文件中缺少某个必需的配置项
    /// </summary>
    /// <param name="message"></param>
    public class ConfigurationNotFoundException(string message) : ConfigurationException(message)
    {
        public override string Code => "CONFIG_NOT_FOUND";
    }

    /// <summary>
    /// 配置文件未找到,用于表示在应用程序自检配置过程中未找到指定的配置文件,
    /// 如配置文件路径错误、配置文件被删除等情况
    /// </summary>
    /// <param name="message"></param>
    public class ConfigurationFileNotFoundException(string message) : ConfigurationException(message)
    {
        public override string Code => "CONFIG_FILE_NOT_FOUND";
    }

    /// <summary>
    /// 配置边界异常,用于表示在应用程序自检配置过程中配置项的值超出预期的边界范围
    /// 例如数值型配置项的值超过了允许的最大值或最小值
    /// </summary>
    /// <param name="message"></param>
    /// 
    public class ConfigurationValueOutOfRangeException(string message) : ConfigurationException(message)
    {
        public override string Code => "CONFIG_VALUE_OUT_OF_RANGE";
    }

    /// <summary>
    /// JSON 配置映射 DTO异常
    /// </summary>
    public class ConfigurationMappingException(string message) : ConfigurationException(message)
    {
        public override string Code => "CONFIG_MAPPING_ERROR";
    }

    /// <summary>
    /// 配置无效或不匹配,主要用于表达配置类型不匹配
    /// </summary>
    /// <param name="message"></param>
    public class ConfigurationInvalidException(string message) : ConfigurationException(message)
    {
        public override string Code => "CONFIG_INVALID";
    }
}

