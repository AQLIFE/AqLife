using MyLife.Core.Define;

namespace MyLife.Core.API
{
    public enum StorageUnit
    {
        NoLimit = 0,
        Byte = 1,
        KB = 2,
        MB = 3,
        GB = 4
    }
    public class FileSecurityPolicy : IFilePolicy
    {
        public FileSecurityPolicy(string defaultSavePath, int maxFileSize, StorageUnit unit, string[] fileTypes, bool allowUpload, bool allowDownload, bool allowDelete)
        {
            AllowedExtensions = fileTypes ?? throw new Exception("FileTypes cannot be null");
            StoragePath = defaultSavePath ?? throw new Exception("DefaultSavePath cannot be null");
            AllowUpload = allowUpload;
            AllowDownload = allowDownload;
            AllowDelete = allowDelete;
            MaxFileSize = maxFileSize;
            StorageUnit = unit;
            // compute bytes as long
            MaxBites = maxFileSize * (long)Math.Ceiling(Math.Pow(1024, (int)unit));
        }

        public string[] AllowedExtensions { init; get; }
        public string StoragePath { init; get; }
        public bool AllowUpload { init; get; }
        public bool AllowDownload { init; get; }
        public bool AllowDelete { init; get; }
        public int MaxFileSize { init; get; }
        public StorageUnit StorageUnit { get; init; }
        public long MaxBites { init; get; }
    }
    public class FilePolicyOptions : IFilePolicy
    {
        public int MaxFileSize { get; init; }
        public string[] AllowedExtensions { get; init; } = new string[0];
        public string StoragePath { get; init; } = string.Empty;
        public StorageUnit StorageUnit { get; init; }
        public bool AllowUpload { get; init; }
        public bool AllowDownload { get; init; }
        public bool AllowDelete { get; init; }

        // Map options to domain FileSecurityPolicy
        public FileSecurityPolicy ToDomain()
        {
            return new FileSecurityPolicy(StoragePath, (int)MaxFileSize, StorageUnit, AllowedExtensions, AllowUpload, AllowDownload, AllowDelete);
        }
    }

}