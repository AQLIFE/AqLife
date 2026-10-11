using AqLife.Shared.Options;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.CoreTest.TestData
{
    public static class FilePolicyTestData
    {
        // 首次访问时读取配置，后续测试复用同一份配置对象。
        private static readonly Lazy<FilePolicyOption> FilePolicy =
            new(LoadFilePolicy);

        private static FilePolicyOption LoadFilePolicy()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    Path.Combine("Configurations", "FilePolicy.json"),
                    optional: false,
                    reloadOnChange: false)
                .Build();
            

            return configuration
                .GetSection("FilePolicy")
                .Get<FilePolicyOption>()
                ?? throw new InvalidOperationException(
                    "测试配置缺少 FilePolicy 节点。");
        }

        // 为 MemberData 提供图片扩展名测试数据。
        public static IEnumerable<object[]> LoadImageExtensions()
        {
            var extensions = FilePolicy.Value.AllowedImageExtensions;

            if (extensions.Length == 0)
            {
                throw new InvalidOperationException(
                    "测试配置中 AllowedImageExtensions 不能为空。");
            }

            return extensions
                .Select(extension => new object[] { extension });
        }
        public static IEnumerable<object[]> LoadBlogExtensions()
        {
            var extensions = FilePolicy.Value.AllowedBlogExtensions;

            if (extensions.Length == 0)
            {
                throw new InvalidOperationException(
                    "测试配置中 AllowedBlogExtensions 不能为空。");
            }

            return extensions
                .Select(extension => new object[] { extension });
        }

        public static IEnumerable<object[]> LoadAllExtensions()
        {
            var extensions = FilePolicy.Value.AllowedExtensions;

            if (extensions.Length == 0)
            {
                throw new InvalidOperationException(
                    "测试配置中 AllowedExtensions 不能为空。");
            }

            return extensions
                .Select(extension => new object[] { extension });
        }

        public static long MaxSize=> FilePolicy.Value.MaxFileSize <<FilePolicy.Value.StorageUnit ;

        public static TheoryData<long> AbnormalFileSize =>
        [
            -1L,
            0L,
            (MaxSize + 1)
        ];

        public static TheoryData<long> LegalBoundarySize =>
        [
            1L,
            (MaxSize - 1),
            MaxSize
        ];

    }
}
