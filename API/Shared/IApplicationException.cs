using MyLife.Shared.Options;

namespace MyLife.Shared
{
    public interface IApplicationException
    {
        ExceptionCategory Category { get; }

        ExceptionBehavior Behavior { get; }
        string Code { get; }
    }
}
