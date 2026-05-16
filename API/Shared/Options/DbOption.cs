namespace MyLife.Shared.Options
{
    public enum BehavioralLevel { DbType, ConfigType, ApiType, FunType }

    public record DbOption(string DbType, string DbVersion);
}
