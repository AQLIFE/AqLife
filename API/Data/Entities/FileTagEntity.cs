using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Data.Entities
{
    [Table("FileTags")]
    public class FileTagEntity
    {
        [ForeignKey(nameof(FileId))]
        public Guid FileId { get; set; } // 指向 FileMetaEntity.Uuid

        [ForeignKey(nameof(TagId))]
        public Guid TagId { get; set; } // 指向 TagEntity.TagId
        // 设定策略,每次更新 md 文件时,仅允许一条分类+若干标签

        public virtual FileMetaEntity File { get; set; } = null!;
        public virtual TagEntity Tag { get; set; } = null!;
    }
}
