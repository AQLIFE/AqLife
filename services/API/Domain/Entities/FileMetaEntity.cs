using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
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
        public string FileHash { private set; get; } = string.Empty;
        [Column]
        public DateTimeOffset UploadTime { private set; get; } = DateTimeOffset.UtcNow;
        [Column]
        public DateTimeOffset? PublishAt { get; private set; } = null;

        [Column]
        public FileStatus PublishStatus { private set; get; } = FileStatus.Draft;

        [NotMapped]
        public string StorageName => UID + Extension;
        [NotMapped]
        public string FileIntroduction { private set; get; } = string.Empty;

        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public FileMetaEntity() { }

        /// <summary>
        /// 构造草稿文件
        /// </summary>
        /// <param name="file">源文件</param>
        public FileMetaEntity(IFormFile file, string hash)
        {
            FileName = Path.GetFileNameWithoutExtension(file.FileName) ?? string.Empty;
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant() ?? string.Empty;
            FileSize = (ulong)file.Length;
            FileHash = hash;
        }
        /// <summary>
        /// 构造预定文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="hash"></param>
        /// <param name="ScheduledTime"></param>
        public FileMetaEntity(IFormFile file, string hash, DateTimeOffset ScheduledTime) : this(file, hash)
        {
            PublishAt = ScheduledTime;
            this.PublishStatus = FileStatus.Scheduled;
        }


        public void Publish()
        {
            if (PublishStatus is not (FileStatus.Draft or FileStatus.Scheduled))
            {
                throw new DomainLegalityException($"Cannot publish.");
            }
            PublishStatus = FileStatus.Published;
            this.PublishAt = DateTimeOffset.UtcNow;
        }

        public void Schedule(DateTimeOffset scheduledTime)
        {
            if (PublishStatus != FileStatus.Draft)// 只有草稿状态的文件才能被预定
            {
                throw new DomainLegalityException("Only draft posts can be scheduled.");
            }

            if (scheduledTime <= DateTimeOffset.UtcNow)// 预定时间必须在当前时间之后
            {
                throw new DomainLegalityException("Scheduled time must be in the future.");
            }
            if (PublishStatus == FileStatus.Published)
                return;

            PublishAt = scheduledTime;
            PublishStatus = FileStatus.Scheduled;
        }

        public void CancelSchedule()
        {
            if (PublishStatus == FileStatus.Draft) return;
            PublishAt = null;
            PublishStatus = FileStatus.Draft;
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
