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
    public class CreateFileMeta(IntegrationTestFixture fixture) : FileTest(fixture)
    {
        private readonly MemoryStream memoryStream = new(new byte[FilePolicyTestData.MaxSize]);
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
        ///  上传模板文件成功
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 上传超限大小的文件,触发报错
        /// </summary>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [MemberData(nameof(FilePolicyTestData.AbnormalFileSize), MemberType = typeof(FilePolicyTestData))]
        public async Task WhenExceedsSize_ShouldThrow(long size)
        {
            FormFile file = new(new MemoryStream(new byte[size<=0?0:size]), 0, size, "testfile", "testfile.md");

            CreateFileCommand create = new(false, [file]);
            var exception=await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(create));
            Assert.Contains("文件大小超出限制", exception.Message);
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

            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", fileName: "testfile" + ext);

            CreateFileCommand create = new(true, [file]);

            var exception=  await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(create));
            Assert.Equal("非 md 文件,禁止标记为模板文件", exception.Message);
        }

        /// <summary>
        /// 上传异常Files的命令,触发报错
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [MemberData(nameof(FilePolicyTestData.ExcetionCommand),MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldExceptionCommand(CreateFileCommand command)
        {
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(command));
            Assert.Equal("非 md 文件,禁止标记为模板文件", exception.Message);
        }

        /// <summary>
        /// 上传空文件报错
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UploadFile_ShouldEmptyFileCommand()
        {
            CreateFileCommand command = new(false, []);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(command));
            Assert.Equal("禁止空上传", exception.Message);
        }
        /// <summary>
        /// 上传重复文件报错
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UploadFile_ShouldRepeatFileCommand()
        {
            CreateFileCommand command = new(false, [
                new FormFile(memoryStream,0,memoryStream.Length,"TETS","TEST.md"),
                new FormFile(memoryStream,0,memoryStream.Length,"TETS","TEST.md")
                ]);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(command));
            Assert.Equal("文件重复", exception.Message);
        }

        /// <summary>
        /// 验证上传错误的扩展名失败
        /// </summary>
        /// <returns></returns>
        [MemberData(nameof(FilePolicyTestData.UnValidExtensions),MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ShouldFailExtension(string ext)
        {
            FormFile file = new(memoryStream, 0, memoryStream.Length, "testfile", "testfile" + ext);

            CreateFileCommand create = new(false, [file]);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await UploadAsync(create));
            Assert.Equal("不允许上传的文件类型", exception.Message);
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
