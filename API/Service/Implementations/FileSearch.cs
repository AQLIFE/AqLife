using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;

namespace MyLife.Service.Implementations
{
    public interface IFileSearchProvider
    {
        Task<FileIndexEntity?> FindFileAsync(string? title, Guid? id);
    }
    public class FileSearch(IEnumerable<IFileSearchStrategy> strategies, AppStorage storage) : IFileSearchProvider
    {
        public async Task<FileIndexEntity?> FindFileAsync(string? title, Guid? id)
        {
            var strategy = strategies.FirstOrDefault(s => s.IsMatch(title, id));
            if (strategy == null) return null;
            return await strategy.ExecuteAsync(storage.File.AsQueryable(), title, id);
        }
    }
}
