namespace MyLife.Shared.Contracts
{
    public interface IUserEntity
    {
        Guid UID { get; }
        string Name { get; }
        // 可以根据需要扩展 Role 等字段
    }

}
