namespace MyLife.Shared
{
    public interface IBaseUser
    {
        Guid UID { get; }
        string Name { get; }
        // 可以根据需要扩展 Role 等字段
    }
}
