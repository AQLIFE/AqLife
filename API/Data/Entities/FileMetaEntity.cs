using Microsoft.AspNetCore.Http;
using MyLife.Shared.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entities
{
    [Table("FileMeta")]
    public class FileMetaEntity : IEntity
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();
        [Column, Description("仅存储文件名,不含后缀")]
        public string FileName { get; set; } = string.Empty;
        [Column]
        public string Extension { get; set; } = string.Empty;
        [Column]
        public ulong FileSize { set; get; } = 0u;

        [Column, Required(ErrorMessage = "文件哈希不能为空")]
        public string FileHash { set; get; }
        [Column]
        public DateTime UploadTime { set; get; } = DateTime.UtcNow;

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
    }
}
