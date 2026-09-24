using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AqLife.Domain.Entities.File
{
    [Table("FileMeta")]
    public class FileMetaEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();
        [Column, Description("仅存储文件名,不含后缀"), StringLength(64)]
        public string FileName { get; private set; } = string.Empty;
        [Column]
        public string Extension { get; private set; } = string.Empty;
        [Column]
        public ulong FileSize { private set; get; } = 0u;

        [Column, Required(ErrorMessage = "文件哈希不能为空")]
        public string FileHash { private set; get; } = string.Empty;
        [Column]
        public DateTimeOffset UploadTime { private set; get; } = DateTimeOffset.UtcNow;
       

        [NotMapped]
        public string StorageKey => UID + Extension;
        [NotMapped]
        public string FileIntroduction { private set; get; } = string.Empty;

        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public virtual required InteractionMetaEntity InteractionMeta { get; set; }
        public virtual required PublishMetaEntity PublishMeta { get; set; }

        [SetsRequiredMembers]
        public FileMetaEntity() {
            InteractionMeta = new InteractionMetaEntity(this.UID);
            PublishMeta = new PublishMetaEntity(this.UID);
        }

        [SetsRequiredMembers]
        public FileMetaEntity(IFormFile file) : this()
        {
            FileName = Path.GetFileNameWithoutExtension(file.FileName) ?? string.Empty;
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant() ?? string.Empty;
            FileSize = (ulong)file.Length;
        }

        /// <summary>
        /// 构造草稿文件
        /// </summary>
        /// <param name="file">源文件</param>
        /// <param name="hash">文件哈希</param>
        [SetsRequiredMembers]
        public FileMetaEntity(IFormFile file, string hash):this(file)
        {
            FileHash = hash;
        }
        

        public void SetFileIntroduction(string content)
        {
            FileIntroduction = content;
        }
        public FileMetaEntity UpdateHash(string hash)
        {
            FileHash = hash;
            return this;
        }
    }
}
