using AqLife.Domain.Contracts;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
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
        public string FileHash { private set; get; }
        [Column]
        public DateTime UploadTime { private set; get; } = DateTime.UtcNow;
        [Column]
        public DateTime? PublishAt { get; private set; } = null;

        [Column]
        public FileStatus Status { private set; get; } = FileStatus.Draft;

        [NotMapped]
        public string StorageName => UID + Extension;
        [NotMapped]
        public string FileIntroduction { private set; get; } = string.Empty;

        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public FileMetaEntity() { }

        /// <summary>
        /// 快速拼装一个 FileMetaEntity 实例, 通过 IFormFile 获取文件的基本信息, 并生成一个新的脱敏文件名
        /// 需要注意 在后续业务逻辑中 手动设置 FileHash 属性, 因为它需要在文件保存后通过计算得到, 不能在构造函数中完成
        /// </summary>
        /// <param name="file">源文件</param>
        public FileMetaEntity(IFormFile file, string hash)
        {
            FileName = Path.GetFileNameWithoutExtension(file.FileName) ?? string.Empty;
            //DesensitizationName = Guid.NewGuid().ToString();
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant() ?? string.Empty;
            FileSize = (ulong)file.Length;
            FileHash = hash;
        }
        public FileMetaEntity(IFormFile file, string hash, DateTime ScheduledTime) : this(file, hash)
        {
            PublishAt = ScheduledTime;
            this.Status = FileStatus.Scheduled;
        }


        public void Publish()
        {
            this.PublishAt = DateTime.UtcNow;
        }

        public void SetFileIntroduction(string content)
        {
            FileIntroduction = content;
        }
        public void UpdateHash(string hash)
        {
            FileHash = hash;
        }
    }
}
