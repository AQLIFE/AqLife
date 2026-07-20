namespace MyLife.Domain.Contracts;

public interface IValidator<in TSource>
{
    public Task VerifyAsync(TSource source, CancellationToken ct = default);
}
