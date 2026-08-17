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
        BadRequest = 400, // 400
        NotFound = 404,   // 404
        Unauthorized = 401,//401
        Forbidden = 403,  // 403
        Conflict = 409,   // 409
        Retry = 449,      // 
        InternalError = 500// 500
    }
}
