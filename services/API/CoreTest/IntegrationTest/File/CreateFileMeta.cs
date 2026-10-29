using AqLife.CoreTest.TestData;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AqLife.CoreTest.IntegrationTest.File
{
    [Collection<IntegrationTestCollection>]
    public class CreateFileMeta(IntegrationTestFixture fixture) : FileTest(fixture)
    {
        private static MemoryStream CreateMaxSizeStream() => new(new byte[FilePolicyTestData.MaxSize]);
        public static IEnumerable<object[]> AllowedExtensions => FilePolicyTestData.LoadAllExtensions();
        public static IEnumerable<object[]> AllowedImageExtensions => FilePolicyTestData.LoadImageExtensions();
        /// <summary>
        /// 验证上传允许的扩展名成功
        /// </summary>
        /// <returns></returns>
        [MemberData(nameof(AllowedExtensions))]
        [Theory, ResetDatabase]
        public async Task UploadFile_AllowedExtension_ShouldSucceed(string allowedExtension)
        {
            using var stream = CreateMaxSizeStream();
            FormFile file = new(stream, 0, stream.Length, "testfile", "testfile" + allowedExtension);

            CreateFileCommand create = new(false, [file]);
            IEnumerable<Guid> result = await base.ExecuteAsync(create, CancellationToken.None);
            Guid guid = result.FirstOrDefault();
            Assert.NotEqual(Guid.Empty, guid);
            var uploadedFile = await FindFileAsync(guid);
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
        public async Task UploadFile_LegalBoundarySize_ShouldSucceed(long legalSize)
        {
            using var stream = CreateMaxSizeStream();
            FormFile file = new(stream, 0, legalSize, "testfile", "testfile.md");

            CreateFileCommand create = new(false, [file]);
            IEnumerable<Guid> result = await base.ExecuteAsync(create, CancellationToken.None);

            var uploadedFile = await FindFileAsync(result.FirstOrDefault(), CancellationToken.None);
            Assert.NotNull(uploadedFile);
            Assert.Equal(file.Length, (long)uploadedFile.FileSize);
        }

        /// <summary>
        ///  上传模板文件成功
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UploadFile_TemplateMarkdown_ShouldSucceed()
        {
            using var stream = CreateMaxSizeStream();
            FormFile file = new(stream, 0, stream.Length, "testfile", fileName: "testfile" + ".md");
            CreateFileCommand create = new(true, [file]);
            IEnumerable<Guid> result = await ExecuteAsync(create, CancellationToken.None);
            var uploadedFile = await FindFileAsync(result.FirstOrDefault(), CancellationToken.None);
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
        public async Task UploadFile_ExceedsSizeLimit_ShouldReject(long oversizedLength)
        {
            FormFile file = new(new MemoryStream(new byte[Math.Max(oversizedLength, 0)]), 0, oversizedLength, "testfile", "testfile.md");

            CreateFileCommand create = new(false, [file]);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(create, CancellationToken.None));
            Assert.Contains("文件大小超出对应类型的上传限制", exception.Message);
        }
        /// <summary>
        /// 上传非博文文件,但设置模板标记,触发上传拦截报错
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        [MemberData(nameof(AllowedImageExtensions))]
        [Theory, ResetDatabase]
        public async Task UploadFile_ImageWithTemplateFlag_ShouldReject(string imageExtension)
        {
            using var stream = CreateMaxSizeStream();
            FormFile file = new(stream, 0, stream.Length, "testfile", fileName: "testfile" + imageExtension);

            CreateFileCommand create = new(true, [file]);

            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(create, CancellationToken.None));
            Assert.Equal("非 md 文件,禁止标记为模板文件", exception.Message);
        }

        /// <summary>
        /// 上传异常Files的命令,触发报错
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [MemberData(nameof(FilePolicyTestData.InvalidTemplateCommand), MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_InvalidTemplateCommand_ShouldReject(CreateFileCommand command)
        {
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            Assert.Equal("非 md 文件,禁止标记为模板文件", exception.Message);
        }

        /// <summary>
        /// 上传空文件报错
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UploadFile_EmptyFileList_ShouldReject()
        {
            CreateFileCommand command = new(false, []);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            Assert.Equal("禁止空上传", exception.Message);
        }
        /// <summary>
        /// 上传重复文件报错
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UploadFile_DuplicateFileName_ShouldReject()
        {
            using var stream = CreateMaxSizeStream();

            CreateFileCommand command = new(false, [
                new FormFile(stream,0,stream.Length,"TETS","TEST.md"),
                new FormFile(stream,0,stream.Length,"TETS","TEST.md")
                ]);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            Assert.Equal("文件重复", exception.Message);
        }

        /// <summary>
        /// 验证上传错误的扩展名失败
        /// </summary>
        /// <returns></returns>
        [MemberData(nameof(FilePolicyTestData.UnValidExtensions), MemberType = typeof(FilePolicyTestData))]
        [Theory, ResetDatabase]
        public async Task UploadFile_DisallowedExtension_ShouldReject(string disallowedExtension)
        {
            using var stream = CreateMaxSizeStream();
            FormFile file = new(stream, 0, stream.Length, "testfile", "testfile" + disallowedExtension);

            CreateFileCommand create = new(false, [file]);
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(create, CancellationToken.None));
            Assert.Equal("不允许上传的文件类型", exception.Message);
        }

        private async Task<FileMetaEntity?> FindFileAsync(Guid guid, CancellationToken cancellation = default)
            => await base.QueryAsync(e => e.UID == guid, cancellation);
    }
}
