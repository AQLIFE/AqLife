using AqLife.Shared.Options;

namespace AqLife.Shared
{
    public interface IApplicationException
    {
        ExceptionCategory Category { get; }

        ExceptionBehavior Behavior { get; }
        string Code { get; }
    }
}
