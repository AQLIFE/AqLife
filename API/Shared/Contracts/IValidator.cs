namespace MyLife.Shared.Contracts;

public interface IValidator<in TSource>
{
    public Task VerifyAsync(TSource source, CancellationToken ct = default);
}
