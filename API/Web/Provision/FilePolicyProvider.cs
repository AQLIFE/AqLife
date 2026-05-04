using MyLife.Shared.Config;
using Serilog;
using MyLife.Shared.Accident;

namespace MyLife.Web.Provision
{
    public static class FilePolicyProvider
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
            Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, "正在绑定文件存储策略...");

            var section = builder.Configuration.GetSection("FilePolicy");
            builder.Services.AddOptions<FilePolicy>().Bind(section).ValidateOnStart();

            var filePolicy = section.Get<FilePolicy>() ?? throw new OptionMappingException("配置文件中缺失 FilePolicy 节点或 StoragePath 设置");

            EnsureStorageDirectoryCreated(builder.Environment.ContentRootPath, filePolicy.StoragePath);
            return builder;

            //builder.Services.Configure<FilePolicy>(section); ## 延迟加载, 不能在启动时触发验证,仅在第一次注入时触发验证
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="path"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OptionSelfRecoveryMeasuresException"></exception>
        public static void EnsureStorageDirectoryCreated(string rootPath,string path)
        {
            // 1. 基础验证：防止传入空路径导致崩溃
            if (string.IsNullOrWhiteSpace(rootPath) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("存储根路径或子路径不能为空。");
            }
            var fullPath = Path.Combine(rootPath, path);

            if (Directory.Exists(fullPath))
            {
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, "文件策略初始化完成.");
            }
            else
            {
                Log.Warning(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, "存储路径不存在，尝试自动创建...");
                try
                {
                    Directory.CreateDirectory(fullPath); // 自动创建
                    Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, $"存储路径创建成功: {fullPath}");
                }
                catch
                {
                    Log.Error(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ConfigType, "存储路径恢复失败,请检查权限或磁盘状态.");
                    throw new OptionSelfRecoveryMeasuresException("存储路径恢复失败,请检查权限或磁盘状态.");
                }
            }
        }
    }
}
