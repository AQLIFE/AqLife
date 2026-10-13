using Swashbuckle.AspNetCore.Annotations;

namespace AqLife.Shared.Options
{
    [SwaggerSchema("待办状态")]
    public enum TodoStatus
    {
        Initial,
        Wait,
        
        Execute,
        
        Completed
    }
}
