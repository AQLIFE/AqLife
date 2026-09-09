using AqLife.Shared.Options;
using Serilog;

namespace AqLife.Web.Extensions
{
    public static class FilePolicyInitializer
    {
        /// <summary>
        /// 添加 文件存储策略配置,并在启动时验证配置的有效性,如果配置无效则抛出异常阻止应用启动
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="OptionMappingException"></exception>
        /// <exception cref="OptionNotFoundException"></exception>
        public static WebApplicationBuilder AddFilePolicy(this WebApplicationBuilder builder)
        {
            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "正在绑定文件存储策略...");

            var section = builder.Configuration.GetSection("FilePolicy");

            var filePolicy = section.Get<FilePolicyOption>() ?? throw new ConfigurationNotFoundException("配置文件中缺失 FilePolicy 节点或 StoragePath 设置");

            EnsureStorageDirectoryCreated(builder.Environment.ContentRootPath, filePolicy.StoragePath);
            builder.Services.AddOptions<FilePolicyOption>().Bind(section).ValidateOnStart();
            return builder;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OptionSelfRecoveryMeasuresException"></exception>
        public static void EnsureStorageDirectoryCreated(string rootPath, string path)
        {
            // 1. 基础验证：防止传入空路径导致崩溃
            if (string.IsNullOrWhiteSpace(rootPath) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("存储根路径或子路径不能为空。");
            }
            var fullPath = Path.Combine(rootPath, path);

            if (Directory.Exists(fullPath))
            {
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "文件策略初始化完成.");
            }
            else
            {
                Log.Warning(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "存储路径不存在，尝试自动创建...");
                try
                {
                    Directory.CreateDirectory(fullPath); // 自动创建
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, $"存储路径创建成功: {fullPath}");
                }
                catch
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.OptionType, "存储路径恢复失败,请检查权限或磁盘状态.");
                    throw new Exception("存储路径恢复失败,请检查权限或磁盘状态.");//待定
                }
            }
        }
    }
}
