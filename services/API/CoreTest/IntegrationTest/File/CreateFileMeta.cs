using Amazon.Runtime.Internal.Util;
using AqLife.CoreTest.TestData;
using AqLife.Domain.Command;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AqLife.CoreTest.IntegrationTest.File
{
    [Collection<IntegrationTestCollection>]
    public class CreateFileMeta(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
    {
        private readonly MemoryStream memoryStream = new(Encoding.UTF8.GetBytes("测试文件内容"));
        public static IEnumerable<object[]> AllowedExtensions => FilePolicyTestData.LoadAllExtensions();
        public static IEnumerable<object[]> AllowedImageExtensions => FilePolicyTestData.LoadImageExtensions();
        /// <summary>
        /// 验证上传允许的扩展名成功
        /// </summary>
        /// <returns></returns>
        [MemberData(nameof(AllowedExtensions))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldAllowedExtension(string ext)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", "testfile" + ext);

            CreateFileCommand create = new(false, [file]);
            IEnumerable<Guid> result = await mediator.Send(create, CancellationToken.None);

            var uploadedFile = await storage.File.SingleOrDefaultAsync(f => f.UID == result.First(), CancellationToken.None);
            Assert.NotNull(uploadedFile);
            Assert.Equal(file.FileName, uploadedFile.FileName + uploadedFile.Extension);
            Assert.Equal(file.Length, (long)uploadedFile.FileSize);
        }

        /// <summary>
        /// 验证上传错误的扩展名失败
        /// </summary>
        /// <returns></returns>
        [InlineData(".md1")]
        [InlineData(".1png")]
        [InlineData(".png.exe")]
        [InlineData(".pn")]
        [InlineData(".jeg")]
        [InlineData(".gif")]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldFailExtension(string ext)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", "testfile" + ext);

            CreateFileCommand create = new(false, [file]);
            await Assert.ThrowsAsync<RequestCheckException>(async () => await mediator.Send(create, CancellationToken.None));
        }
        /// <summary>
        /// 上传超限大小的文件,触发报错
        /// </summary>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [MemberData(nameof(FilePolicyTestData.AbnormalFileSize), MemberType = typeof(FilePolicyTestData))]
        public async Task WhenExceedsSize_ShouldThrow(long size)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            FormFile file = new(memoryStream, 0, size, "testfile", "testfile.md");

            CreateFileCommand create = new(false, [file]);
            await Assert.ThrowsAsync<RequestCheckException>(async () => await mediator.Send(create, CancellationToken.None));
        }
        /// <summary>
        /// 合法文件大小上传成功
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [MemberData(nameof(FilePolicyTestData.LegalBoundarySize), MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldAllowedSizeFile(long size)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            FormFile file = new(memoryStream, 0, size, "testfile", "testfile.md");

            CreateFileCommand create = new(false, [file]);
            IEnumerable<Guid> result = await mediator.Send(create, CancellationToken.None);

            var uploadedFile = await storage.File.SingleOrDefaultAsync(f => f.UID == result.First(), CancellationToken.None);
            Assert.NotNull(uploadedFile);
            Assert.Equal(file.Length, (long)uploadedFile.FileSize);
        }

        /// <summary>
        /// 上传非博文文件,但设置模板标记,触发上传拦截报错
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        [MemberData(nameof(AllowedImageExtensions))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldTemplateFile(string ext)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", fileName:"testfile" + ext);

            CreateFileCommand create = new(true, [file]);

            await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(create));
        }

        [Fact,ResetDatabase]
        public async Task UploadFile_ShouldTemplateFile_Success()
        {
            using var scope = CreateScope();
            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();
            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", fileName: "testfile"+".md");
            CreateFileCommand create = new(true, [file]);
            IEnumerable<Guid> result = await UploadAsync(create);
            var uploadedFile = await storage.File.SingleOrDefaultAsync(f => f.UID == result.First(), CancellationToken.None);
            Assert.NotNull(uploadedFile);
            Assert.Equal(file.FileName, uploadedFile.FileName + uploadedFile.Extension);
            Assert.True(uploadedFile.IsTemplate);
            Assert.Equal(".md", uploadedFile.Extension);
        }

        [MemberData(nameof(FilePolicyTestData.ExcetionCommand),MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldExceptionCommand(CreateFileCommand command)
        {
            using var scope = CreateScope();
            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(command));
        }

        private async Task<IEnumerable<Guid>> UploadAsync(CreateFileCommand command)
        {
            using var scope = CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            // 注意：memoryStream 长度要和 length 一致，或按你项目里真实构造 FormFile 的方式
            return await mediator.Send(command, CancellationToken.None);
        }
    }
}
