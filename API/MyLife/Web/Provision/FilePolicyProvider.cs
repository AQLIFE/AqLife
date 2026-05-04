using MyLife.Shared.Config;
using Serilog;

namespace MyLife.Web.Provision
{
    public static class FilePolicyProvider
    {
        /// <summary>
        /// 在builder 中注册 FilePolicy 配置,并在启动时验证配置的正确性,如果配置不正确则抛出异常,并记录日志
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static WebApplicationBuilder BindFilePolicy(this WebApplicationBuilder builder)
        {
            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "正在绑定文件存储策略...");

            var section = builder.Configuration.GetSection("FilePolicy");
            builder.Services.AddOptions<FilePolicy>().Bind(section).ValidateOnStart();

            var filePolicy = section.Get<FilePolicy>() ?? throw new Exception("Not found FilePolicy!");

            //builder.Services.Configure<FilePolicy>(section); ## 延迟加载, 不能在启动时触发验证,仅在第一次注入时触发验证
            if (filePolicy != null && !string.IsNullOrEmpty(filePolicy.StoragePath))
            {
                // 执行 IO 相关的初始化工作
                EnsureStorageDirectoryCreated(builder.Environment.ContentRootPath, filePolicy.StoragePath);
            }
            else
            {
                throw new Exception("配置文件中缺失 FilePolicy 节点或 StoragePath 设置");
            }
            return builder;
        }



        /// <summary>
        /// 安全检查目录,不存在则创建,存在则记录日志
        /// </summary>
        /// <param name="env">环境根路径</param>
        /// <param name="path">目标目录</param>
        public static void EnsureStorageDirectoryCreated(string rootPath, string path)
        {
            var fullPath = Path.Combine(rootPath, path);

            if (Directory.Exists(fullPath))
            {
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "文件策略初始化完成.");
            }
            else
            {
                Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ConfigType, "文件策略初始化失败，存储路径不存在.");
                Log.Warning(@"[Serilog]=> 存储路径不存在，正在尝试创建: {Path}", fullPath);
                Directory.CreateDirectory(fullPath); // 自动创建
                Log.Information(@"[Serilog]=> 存储路径创建成功: {Path}", fullPath);
            }
        }
    }
}
