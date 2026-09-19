namespace AqLife.Application.Abstractions.FileStorage
{
    public interface IFileStorage
    {
        Task SaveAsync(Stream content, string key, CancellationToken ct =default);

        Task<bool> DeleteAsync(string key, CancellationToken ct = default);

        //Task<bool> ExistsAsync(string key, CancellationToken ct = default);

        Task<Stream> OpenReadAsync(string key, CancellationToken ct = default);
    }
}
