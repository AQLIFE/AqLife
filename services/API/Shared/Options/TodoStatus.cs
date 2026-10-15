using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace AqLife.Shared.Options
{
    [SwaggerSchema("待办状态")]
    public enum TodoStatus
    {
        [Description("初始状态")]
        Initial = 0,
        [Description("等待分配")]
        Wait = 1,
        [Description("正在执行")]
        Execute = 2,
        [Description("执行完成")]
        Completed =3
    }
}
