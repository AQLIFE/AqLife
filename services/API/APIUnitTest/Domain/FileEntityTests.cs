using Microsoft.AspNetCore.Http;
using MyLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUnitTest.Domain
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
        }
    }
}
