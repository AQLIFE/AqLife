using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
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
        public string Extension { get; init; } = string.Empty;
        [Column]
        public ulong FileSize { private set; get; } = 0u;

        [Column, Required(ErrorMessage = "文件哈希不能为空")]
        public string FileHash { private set; get; } = string.Empty;
        [Column]
        public DateTimeOffset UploadTime { get; init; } = DateTimeOffset.UtcNow;

        [Column, ConcurrencyCheck]
        public int Version { get; private set; } = 1;
        [Column]
        public bool IsTemplate { get; init; } = false;




        [NotMapped]
        public string StorageKey => UID + Extension;
        [NotMapped]
        public string FileIntroduction { private set; get; } = string.Empty;

        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public virtual required InteractionMetaEntity InteractionMeta { get; set; } = null!;
        public virtual required PublishMetaEntity PublishMeta { get; set; } = null!;

        [Column]
        public Guid CategoryUID { get; private set; } = Guid.Empty;

        [ForeignKey(nameof(CategoryUID))]

        public virtual CategoryEntity Category { get; set; } = null!;
        [SetsRequiredMembers]
        public FileMetaEntity() { }

        [SetsRequiredMembers]
        [MapperConstructor]
        public FileMetaEntity(IFormFile file, Guid categoryId, string? hash = null,bool isTemplate = false) : this()
        {
            FileName = Path.GetFileNameWithoutExtension(file.FileName) ?? "未命名文件";
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            FileSize = (ulong)file.Length;
            InteractionMeta = new(this.UID);
            PublishMeta = new(this.UID);
            SetCategory(categoryId);
            FileHash = hash ?? string.Empty;
            this.IsTemplate = isTemplate;
        }

        /// <summary>
        /// 不设置即使用默认分类
        /// </summary>
        /// <param name="categoryUID"></param>
        /// <returns></returns>
        public FileMetaEntity SetCategory(Guid? categoryUID = null)
        {
            CategoryUID = categoryUID ?? CategoryEntity.DefaultCategoryId;
            return this;
        }

        /// <summary>
        /// 仅适用于 更新文件,更新后该记录自动增加
        /// </summary>
        public void IncrementVersion()
        {
            checked
            {
                Version++;
            }
        }
        /// <summary>
        /// 设置 文件速览内容,不允许将文件所有内容纳入其中,若要传输所有内容请使用FileReader
        /// </summary>
        /// <param name="content"></param>
        public void SetFileIntroduction(string content)
        {
            if (this.Extension == ".md") FileIntroduction = content;// 暂时写死,等待未来优化
            else throw new DomainLegalityException("不允许非博文类型的写入");
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
