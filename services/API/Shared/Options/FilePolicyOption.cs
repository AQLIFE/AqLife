using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Shared.Options
{
    public sealed class FilePolicyOption
    {
        public int MaxFileSize { set; get; } = 0;
        /// <summary>
        /// Maximum video size in units of 2^StorageUnit bytes. Defaults to 1 GiB when StorageUnit is 10.
        /// </summary>
        public int VideoMaxFileSize { set; get; } = 0;
        public int StorageUnit { set; get; } = 0;
        public string StoragePath { set; get; } = string.Empty;

        public long DefaultFileSize => (long)MaxFileSize << StorageUnit;
        public long VideoFileSize => (long)VideoMaxFileSize << StorageUnit;

        public string[] AllowedImageExtensions { set; get; } = [];
        public string[] AllowedBlogExtensions { set; get; } = [];
        public string[] AllowedVideoExtensions { set; get; } = [];

        public string[] AllowedUpload { set; get; } = [];
        public string[] AllowedDownload { set; get; } = [];

        public string[] AllowedExtensions => [.. AllowedBlogExtensions, .. AllowedImageExtensions, .. AllowedVideoExtensions];

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
    /// 文件浏览范围,Default 包含所有允许公开展示的文件类型
    /// </summary>
    public enum FileScope
    {
        [Description("默认搜索策略，包含所有公开文件类型")]
        Default = 0,
        [Description("Image 搜索策略")]
        Image = 1,
        [Description("Blog 搜索策略")]
        Blog = 2,
        [Description("模板搜索策略")]
        Template = 3,
        [Description("全集搜索策略")]
        All = 4,
        [Description("Video 搜索策略")]
        Video = 5
    }
}