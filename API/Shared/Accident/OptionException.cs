using MyLife.Shared.Options;

namespace MyLife.Shared.Accident
{
    /// <summary>
    /// 终端配置异常,用于表示在终端自检配置过程中发生的异常,例如配置项缺失、配置值无效等情况,可以在全局异常处理中捕获并进行相应的处理,如记录日志、提示用户等
    /// </summary>
    /// <param name="message"></param>
    public abstract class OptionException(string message) : BusinessException(message), IBusinessException
    {
        public override BehavioralLevel Level { get; set; } = BehavioralLevel.DbType;
    }

    /// <summary>
    /// 配置项未找到异常,用于表示在终端自检配置过程中未找到指定的配置项
    /// 
    /// 例如配置文件中缺少某个必需的配置项
    /// </summary>
    /// <param name="message"></param>
    public class OptionNotFoundException(string message) : OptionException(message)
    {
    }

    /// <summary>
    /// 配置文件未找到,用于表示在终端自检配置过程中未找到指定的配置文件,
    /// 如配置文件路径错误、配置文件被删除等情况
    /// </summary>
    /// <param name="message"></param>
    public class OptionFileNotFoundException(string message) : OptionException(message)
    {
    }

    /// <summary>
    /// 配置边界异常,用于表示在终端自检配置过程中配置项的值超出预期的边界范围
    /// 例如数值型配置项的值超过了允许的最大值或最小值
    /// </summary>
    /// <param name="message"></param>
    public class OptionBoundaryException(string message) : OptionException(message)
    {
    }

    /// <summary>
    /// JSON 配置映射 DTO异常
    /// </summary>
    public class OptionMappingException(string message) : OptionException(message) { }

    /// <summary>
    /// 配置自检恢复异常,用于表示在终端自检配置过程中尝试进行恢复操作时发生的异常,例如尝试创建缺失的配置文件或目录时发生的权限异常等情况
    /// </summary>
    /// <param name="message"></param>
    public class OptionSelfRecoveryMeasuresException(string message) : OptionException(message) { }

    /// <summary>
    /// 配置无效或不匹配
    /// </summary>
    /// <param name="message"></param>
    public class OptionInvalidException(string message) : OptionException(message) { }
}

