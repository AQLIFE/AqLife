namespace MyLife.Shared.Config
{
    public class FileOption
    {
        public int MaxFileSize { set; get; }
        public string[] AllowedExtensions { set; get; } = Array.Empty<string>();

        public int StorageUnit { set; get; } = 0;
        public string StoragePath { set; get; } = string.Empty;

        public bool AllowUpload { set; get; } = false;
        public bool AllowDelete { set; get; } = false;
        public bool AllowDownload { set; get; } = false;

        public FileOption() { }
    }
}
