using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace AqLife.CoreTest.Domain
{
    public class FileEntityTests
    {
        [Fact]
        public void Constructor_ShouldCreateFileMetadata()
        {
            // Arrange
            var content = new MemoryStream(
                Encoding.UTF8.GetBytes("Hello World")
            );

            IFormFile file = new FormFile(
                content,
                0,
                content.Length,
                "file",
                "hello.md"
            );

            const string hash = "test-hash";

            // Act
            var entity = new FileMetaEntity(file, hash);

            // Assert
            Assert.Equal("hello", entity.FileName);
            Assert.Equal(".md", entity.Extension);
            Assert.Equal((ulong)content.Length, entity.FileSize);
            Assert.Equal(hash, entity.FileHash);
            Assert.Equal(1, entity.Version);
            Assert.False(entity.IsTemplate);
        }

        [Fact]
        public void TemplateAndVersion_ShouldBeManagedByEntity()
        {
            var content = new MemoryStream(Encoding.UTF8.GetBytes("template"));
            IFormFile file = new FormFile(content, 0, content.Length, "file", "template.md");
            var entity = new FileMetaEntity(file, "template-hash");

            entity.SetTemplate();
            entity.IncrementVersion();

            Assert.True(entity.IsTemplate);
            Assert.Equal(2, entity.Version);
        }
    }
}
