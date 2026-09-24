using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Domain.Entities.File
{
    //设计上 1:1
    public class PublishMetaEntity: IEntity
    {
        [Key]   
        public Guid UID { get; init; } = Guid.Empty;
        [Column]
        public DateTimeOffset? PublishAt { get; private set; } = null;

        [Column]
        public FileStatus PublishStatus { private set; get; } = FileStatus.Draft;

        public PublishMetaEntity() { }
        public PublishMetaEntity(Guid uID)
        {
            UID = uID;
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
    }
}
