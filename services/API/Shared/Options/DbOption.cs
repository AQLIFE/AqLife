namespace MyLife.Shared.Options
{
    public enum BehavioralLevel { DbType, OptionType, ValidType, ApiType }

    public record DbOption(string DbType, string DbVersion);
    public enum ExceptionCategory
    {
        Validation,
        Resource,
        Configuration,
        Authentication,
        Authorization,
        Conflict,
        Operation,
        Database,
        ExternalService,
        Infrastructure
    }
    public enum ExceptionBehavior
    {
        BadRequest = 400, // 400 : 错误请求
        NotFound = 404,   // 404 : 未知请求
        Unauthorized = 401,//401 : 无身份请求
        Forbidden = 403,  // 403 : 无权请求
        Conflict = 409,   // 409 : 冲突请求
        Retry = 449,      // 449 : 资源尚在业务内部合规性流程,请稍后再试
        InternalError = 500// 500 : 服务器异常
    }
}
