namespace MyLife.Core.Define
{
    public interface IFileServer
    {
        string UploadFile(byte[] fileData, string fileName);
        byte[] DownloadFile(string fileUrl);
        bool DeleteFile(string fileUrl);
    }
}