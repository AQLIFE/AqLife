using Microsoft.Extensions.Options;
using MyLife.Shared.Options;

namespace MyLife.Infrastructure.FileStorage
{
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
            if (Directory.Exists(policy.Value.StoragePath)) Directory.CreateDirectory(policy.Value.StoragePath);
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

        public bool Exists(string fileName) => File.Exists(GetPath(fileName));

        public Stream OpenRead(string fileName)
        {
            string LocalPath = GetPath(fileName);
            if (!File.Exists(LocalPath)) throw new FileNotFoundException("文件丢失");
            return new FileStream(LocalPath, FileMode.Open, FileAccess.Read);
        }
    }
}
