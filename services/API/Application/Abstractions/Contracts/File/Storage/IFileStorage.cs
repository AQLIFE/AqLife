namespace AqLife.Application.Abstractions.Contracts.File.Storage
{
    public interface IFileStorage
    {
        Task SaveAsync(Stream content, string key, CancellationToken ct = default);

        Task<bool> DeleteAsync(string key, CancellationToken ct = default);


        Task<Stream> OpenReadAsync(string key, CancellationToken ct = default);

        Task<FileStorageRangeReadResult> OpenReadRangeAsync(string key, long start, long end, CancellationToken ct = default);
    }
}
