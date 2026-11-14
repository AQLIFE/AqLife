using Microsoft.AspNetCore.Http;

namespace AqLife.Shared.Tools
{
    /// <summary>
    /// 仅用于传递文件hash
    /// </summary>
    public class UploadContext
    {
        public Dictionary<IFormFile, string> FileHashes { get; set; } = [];
        // 用于存储已经写入成功的路径,如果后续存在执行失败,可以读取该属性,并回滚写入
        // 本次请求成功写入的文件：事务失败时清理
        public HashSet<string> SavePath { get; } = [];

        // 本次请求待删除的文件：事务成功后清理
        public HashSet<string> DeletePath { get; } = [];
    }
}
