using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Entity
{
    [Table("File")]
    public class FileIndexEntity
    {
        [Column,Key]
        public Guid Uuid { get; set; } = Guid.NewGuid();
        [Column]
        public string FileName { get; set; } = "default";
        [Column]
        public string? Desc { get; set; } = null;
        [Column]
        public ulong FilSize { set; get; } = 0u;
        [Column]
        public DateTime UploadTime { set; get; } = DateTime.Now;
    }
}
