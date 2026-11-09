namespace AqLife.Application.BackServices
{
    public interface IFilePublishService
    {
        Task<Guid> PublishAsync(Guid guid, CancellationToken ct = default);
        Task<int> PublishAsync(IEnumerable<Guid> guids, CancellationToken ct = default);

        Task<Guid> Agreement(Guid guid, DateTimeOffset agreementTime, CancellationToken ct = default);
    }
}
