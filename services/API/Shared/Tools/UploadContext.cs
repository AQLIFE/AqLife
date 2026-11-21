using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace AqLife.Shared.Tools
{
    /// <summary>
    /// 仅用于传递文件哈希及事务期间的物理文件补偿信息。
    /// </summary>
    public class UploadContext
    {
        public Dictionary<IFormFile, string> FileHashes { get; } = [];

        // 本次请求成功写入的文件：事务失败时清理。
        public HashSet<string> SavePath { get; } = [];

        // 本次请求待删除的文件：事务成功后清理。
        public HashSet<string> DeletePath { get; } = [];

        /// <summary>
        /// 获取文件哈希；若上传管道尚未计算，则在此处惰性计算并缓存。
        /// 这样文件资产契约也可以被非 IHasFormFiles 的应用流程安全调用。
        /// </summary>
        public async Task<string> GetOrAddFileHashAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (FileHashes.TryGetValue(file, out var cachedHash))
                return cachedHash;

            await using var stream = file.OpenReadStream();
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
            var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

            FileHashes[file] = hash;
            return hash;
        }
    }
}
