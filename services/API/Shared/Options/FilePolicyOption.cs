using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Shared.Options
{
    public sealed class FilePolicyOption
    {
        public int MaxFileSize { set; get; } = 0;
        public int StorageUnit { set; get; } = 0;
        public string StoragePath { set; get; } = string.Empty;

        public string[] AllowedImageExtensions { set; get; } = [];
        public string[] AllowedBlogExtensions { set; get; } = [];

        public string[] AllowedUpload { set; get; } = [];
        public string[] AllowedDownload { set; get; } = [];

        public string[] AllowedExtensions => [.. AllowedBlogExtensions, .. AllowedImageExtensions];

        public FilePolicyOption() {}
    }

    public enum FileStatus { Draft, Scheduled, Published }
    public enum FileOrder
    {
        Latest,
        Earliest,
        MostViewed
    }

    /// <summary>
    /// 文件浏览范围,All 仅允许包含 Image、Blog 两种类型的文件,不包含 template 类型的文件
    /// </summary>
    public enum FileScope
    {
        /// <summary>
        /// All 仅允许包含 Image、Blog 两种类型的文件,不包含 template 类型的文件
        /// </summary>
        All,
        Image,
        Blog,
        Template
    }
}
