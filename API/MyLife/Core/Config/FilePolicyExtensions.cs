using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using MyLife.Core.API;
using MyLife.Core.Define;
using MyLife.Core.Provider;

namespace MyLife.Core.Config
{
    public static class FilePolicyExtensions
    {
        public static void AddFilePolicyFromEnvironment(this WebApplicationBuilder builder)
        {
            var filename = $"FilePolicy.{builder.Environment.EnvironmentName}.json";
            var cfgPath = Path.Combine(builder.Environment.ContentRootPath, "Core", "Config", filename);

            if (!File.Exists(cfgPath))
                throw new Exception($"无法读取配置文件: {cfgPath}");

            // Add the json configuration and bind to options
            builder.Configuration.AddJsonFile(cfgPath, optional: true, reloadOnChange: true);
            builder.Services.Configure<FilePolicyOptions>(builder.Configuration);

            // Map options to domain FileSecurityPolicy and register as IFilePolicy
            builder.Services.AddSingleton<IFilePolicy>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<FilePolicyOptions>>().Value;
                return opts.ToDomain();
            });

            // Register provider
            builder.Services.AddSingleton<IFilePolicyProvider, FilePolicyProvider>();
        }
    }
}
