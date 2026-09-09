namespace AqLife.Application.Abstractions.FileStorage
{
    public interface IFileStorage
    {
        Task SaveAsync(Stream content, string fileName, CancellationToken ct);

        Task<bool> DeleteAsync(string fileName, CancellationToken ct);

        bool Exists(string fileName);

        Stream OpenRead(string fileName);
        Task<string> GetContent(string fileName);
    }
}
