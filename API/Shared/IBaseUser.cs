using System.Security.Principal;

namespace MyLife.Shared
{
    public interface IUserEntity
    {
        Guid UID { get; }
        string Name { get; }
        // 可以根据需要扩展 Role 等字段
    }

    public interface ISimpleAccountInfo
    {
        string Name { get; init; }
        string? Desc { get; init; }
    }
}
