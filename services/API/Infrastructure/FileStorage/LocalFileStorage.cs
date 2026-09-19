using AqLife.Application.Abstractions.FileStorage;
using AqLife.Shared.Options;
using Microsoft.Extensions.Options;

namespace AqLife.Infrastructure.FileStorage
{
    /// <summary>
    /// 本地文件存储实现, 主要用于开发环境, 生产环境请使用云存储
    /// </summary>
    /// <param name="policy"></param>
    public class LocalFileStorage(IOptions<FilePolicyOption> policy) : IFileStorage
    {
        private string GetPath(string fileName) => Path.Combine(policy.Value.StoragePath, fileName);

        /// <summary>
        ///  保存文件到本地,并在保存过程中计算文件的哈希值, 避免重复读取文件两次 (一次计算哈希, 一次保存), 提高性能
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task SaveAsync(Stream content, string fileName, CancellationToken ct)
        {
            string LocalPath = GetPath(fileName);
            if (!Directory.Exists(policy.Value.StoragePath)) Directory.CreateDirectory(policy.Value.StoragePath);
            await using var stream = new FileStream(LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(stream, ct);
        }

        public Task<bool> DeleteAsync(string fileName, CancellationToken ct)
        {
            string LocalPath = GetPath(fileName);
            if (!File.Exists(LocalPath)) return Task.FromResult(false);
            File.Delete(LocalPath);
            return Task.FromResult(true);
        }

        //public Task<bool> ExistsAsync(string fileName, CancellationToken ct) => Task.FromResult(File.Exists(GetPath(fileName)));

        public Task<Stream> OpenReadAsync(string fileName, CancellationToken ct)
        {
            string LocalPath = GetPath(fileName);
            if (!File.Exists(LocalPath)) throw new FileNotFoundException("文件丢失");
            return Task.FromResult<Stream>(new FileStream(LocalPath, FileMode.Open, FileAccess.Read));
        }

        [Obsolete("废弃,若需要读取文件内容,请使用 FileReader.GetContent")]

        public async Task<string> GetContent(string fileName, CancellationToken ct)
        {
            string LocalPath = GetPath(fileName);
            if (!File.Exists(LocalPath)) throw new FileNotFoundException("文件丢失");
            return await File.ReadAllTextAsync(LocalPath);
        }
    }
}
