namespace MyLife.Shared.Options
{
    public enum BehavioralLevel { DbType, OptionType, ValidType, ApiType }

    public record DbOption(string DbType, string DbVersion);
}
