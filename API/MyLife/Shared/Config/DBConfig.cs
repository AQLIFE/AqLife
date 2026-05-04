namespace MyLife.Shared.Config
{
    public enum LogType { DbType, ApiType, FunType, ConfigType }

    public class DBConfig
    {
        public string DbType { get; set; }
        public string DbVersion { get; set; }
    }
}
