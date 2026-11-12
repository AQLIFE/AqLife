using AqLife.Shared.Contracts;
using Microsoft.AspNetCore.Http;
using Riok.Mapperly.Abstractions;
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

        [Column, ConcurrencyCheck]
        public int Version { get; private set; } = 1;
        [Column]
        public bool IsTemplate { get; private set; } = false;




        [NotMapped]
        public string StorageKey => UID + Extension;
        [NotMapped]
        public string FileIntroduction { private set; get; } = string.Empty;

        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public virtual required InteractionMetaEntity InteractionMeta { get; set; } = null!;
        public virtual required PublishMetaEntity PublishMeta { get; set; } = null!;

        [Column]
        public Guid? CategoryUID { get; private set; }

        [ForeignKey(nameof(CategoryUID))]

        public virtual CategoryEntity? Category { get; set; } = null;
        [SetsRequiredMembers]
        public FileMetaEntity() { }

        [SetsRequiredMembers]
        [MapperConstructor]
        public FileMetaEntity(IFormFile file, Guid categoryId, string? hash = null) : this()
        {
            FileName = Path.GetFileNameWithoutExtension(file.FileName) ?? string.Empty;
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant() ?? string.Empty;
            FileSize = (ulong)file.Length;
            InteractionMeta = new(this.UID);
            PublishMeta = new(this.UID);
            SetCategory(categoryId);
            FileHash = hash ?? string.Empty;
        }

        public FileMetaEntity SetTemplate(bool isTemplate = true)
        {
            IsTemplate = isTemplate;
            return this;
        }

        public FileMetaEntity SetCategory(Guid? categoryUID)
        {
            CategoryUID = categoryUID;
            return this;
        }

        public void IncrementVersion()
        {
            checked
            {
                Version++;
            }
            //return this;
        }

        public void SetFileIntroduction(string content)
        {
            FileIntroduction = content;
        }
        public FileMetaEntity UpdateHash(string hash)
        {
            FileHash = hash;
            IncrementVersion();
            return this;
        }

        /// <summary>
        /// 调用 导航属性的脱敏处理方法,不允许在事务中使用,仅可在查询中使用(AsNoTracking 子句)
        ///  只负责隐藏 发布时间,不负责隐藏其他信息,如文件名,文件大小等
        /// </summary>
        public FileMetaEntity Desensitize()
        {
            this.PublishMeta.Desensitize();
            return this;
        }
    }
}
