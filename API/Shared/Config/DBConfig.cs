namespace MyLife.Shared.Config
{
    public enum BehavioralLevel { DbType, ConfigType, ApiType, FunType  }

    public class DBConfig
    {
        public string DbType { get; set; }
        public string DbVersion { get; set; }
    }
}
