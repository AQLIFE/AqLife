namespace MyLife.Shared.Options
{
    public class FilePolicyOption
    {
        public int MaxFileSize { set; get; }
        public int StorageUnit { set; get; } = 0;
        public string StoragePath { set; get; } = string.Empty;

        public bool AllowDelete { set; get; } = false;
        public string[] AllowedUpload { set; get; } = [];
        public string[] AllowedDownload { set; get; } = [];

        public FilePolicyOption() { }
    }

    public enum FileStatus{Draft,Scheduled,Published }
}
