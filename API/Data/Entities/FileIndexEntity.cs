using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entities
{
    [Table("File")]
    public class FileIndexEntity
    {
        [Key]
        public Guid Uuid { get; set; } = Guid.NewGuid();
        [Column]
        public string FileName { get; set; } = "default";
        [Column]
        public string DesensitizationName { get; set; } = string.Empty;
        [Column]
        public ulong FileSize { set; get; } = 0u;
        [Column]
        public string FileHash { set; get; } = "default";
        [Column]
        public DateTime UploadTime { set; get; } = DateTime.Now;
    }
}
