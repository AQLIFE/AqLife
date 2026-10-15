using System.ComponentModel;
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
    /// 文件浏览范围,Default 仅允许包含 Image、Blog 两种类型的文件,不包含 template 类型的文件
    /// </summary>
    public enum FileScope
    {
        /// <summary>
        /// Default 仅允许包含 Image、Blog 两种类型的文件,不包含 template 类型的文件
        /// </summary>
        [Description("默认搜索策略,包含 Image & Blog ")]
        Default = 0,
        [Description("Image 搜索策略")]
        Image = 1,
        [Description("Blog 搜索策略")]
        Blog = 2,
        /// <summary>
        /// 模板搜索策略,游客禁用
        /// </summary>
        [Description("Template 搜索策略")]
        Template= 3,
        /// <summary>
        /// 全集搜素,游客禁用
        /// </summary>
        [Description("全集搜索策略")]
        All = 4
    }
}
