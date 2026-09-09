using Microsoft.AspNetCore.Http;

namespace AqLife.Shared.Tools
{
    /// <summary>
    /// 仅用于传递文件hash
    /// </summary>
    public class UploadContext
    {
        public Dictionary<IFormFile, string> FileHashes { get; set; } = new();
    }
}
