using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AqLife.Extensions.Configurations;

public static class FilePolicyExtensions
{
    public static IServiceCollection AddFilePolicy(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        Log.Information(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "正在绑定文件存储策略...");

        var filePolicySection = configuration.GetSection("FilePolicy");
        var r2OptionSection = configuration.GetSection("R2");

        var filePolicy = filePolicySection.Get<FilePolicyOption>()
            ?? throw new ConfigurationNotFoundException(
                "配置文件中缺失 FilePolicy 节点或 StoragePath 设置");

        EnsureStorageDirectoryCreated(contentRootPath, filePolicy.StoragePath);

        services
            .AddOptions<FilePolicyOption>()
            .Bind(filePolicySection)
            .ValidateOnStart();

        services
            .AddOptions<R2Options>()
            .Bind(r2OptionSection)
            .ValidateOnStart();

        return services;
    }

    private static void EnsureStorageDirectoryCreated(
        string rootPath,
        string path)
    {
        if (string.IsNullOrWhiteSpace(rootPath) ||
            string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("存储根路径或子路径不能为空。");
        }

        var fullPath = Path.Combine(rootPath, path);

        if (Directory.Exists(fullPath))
        {
            Log.Information(
                "[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.OptionType,
                "文件策略初始化完成.");

            return;
        }

        Log.Warning(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.OptionType,
            "存储路径不存在，尝试自动创建...");

        try
        {
            Directory.CreateDirectory(fullPath);

            Log.Information(
                "[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.OptionType,
                $"存储路径创建成功: {fullPath}");
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.OptionType,
                "存储路径恢复失败,请检查权限或磁盘状态.");

            throw new ConfigurationInvalidException(
                $"存储路径恢复失败,请检查权限或磁盘状态: {ex.Message}");
        }
    }
}
