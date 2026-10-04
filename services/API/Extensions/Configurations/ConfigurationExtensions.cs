using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AqLife.Extensions.Configurations;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAqLifeConfiguration(
        this IConfigurationBuilder configuration,
        string contentRootPath,
        string environmentName)
    {
        var configFolder = Path.Combine(contentRootPath, "Configurations");

        AddRequiredFile(configuration, configFolder, "appsettings.json");
        AddOptionalFile(configuration, configFolder, $"appsettings.{environmentName}.json");

        AddRequiredFile(configuration, configFolder, "FilePolicy.json");
        AddOptionalFile(configuration, configFolder, $"FilePolicy.{environmentName}.json");


        return configuration;
    }

    private static void AddRequiredFile(
        IConfigurationBuilder configuration,
        string folder,
        string fileName)
    {
        var path = Path.Combine(folder, fileName);

        if (!File.Exists(path))
        {
            Log.Error(
                "[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.OptionType,
                $"配置文件不存在: {path}");

            throw new ConfigurationFileNotFoundException(
                $"核心配置文件不存在: {fileName}");
        }

        configuration.AddJsonFile(path, optional: false, reloadOnChange: true);
    }

    private static void AddOptionalFile(
        IConfigurationBuilder configuration,
        string folder,
        string fileName)
    {
        var path = Path.Combine(folder, fileName);

        if (File.Exists(path))
        {
            configuration.AddJsonFile(path, optional: true, reloadOnChange: true);

            Log.Debug(
                "[Serilog][{@LogType}]=>{@LogDesc}",
                BehavioralLevel.OptionType,
                $"配置文件已载入: {fileName}");
        }
    }
}
