using Serilog;
namespace MyLife.Core.Provider
{
    public class FilePolicy
    {
        public int MaxFileSize { set; get; }
        public string[] AllowedExtensions { set; get; } = Array.Empty<string>();

        public int StorageUnit { set; get; } = 0;
        public string StoragePath { set; get; } = string.Empty;

        public bool AllowUpload { set; get; } = false;
        public bool AllowDelete { set; get; } = false;
        public bool AllowDownload { set; get; } = false;

        public FilePolicy() { }
    }

    public static class FilePolicyProvider
    {
        public static WebApplicationBuilder BindFilePolicy(this WebApplicationBuilder builder)
        {
            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "正在绑定文件存储策略...");

            var section = builder.Configuration.GetSection("FilePolicy");
            var filePolicy = section.Get<FilePolicy>() ?? throw new Exception("Not found FilePolicy!");

            if (Directory.Exists(Path.Combine(builder.Environment.ContentRootPath, filePolicy.StoragePath)))
            {
                builder.Services.AddOptions<FilePolicy>().Bind(section).ValidateOnStart();
                //builder.Services.Configure<FilePolicy>(section);
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "文件策略初始化完成.");
            }
            else
                Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "文件策略初始化失败，存储路径不存在.");

            return builder;
        }

        //public static WebApplicationBuilder BindFilePolicy(this WebApplicationBuilder builder)
        //{

        //}
    }
}
