using AqLife.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("Tags"), Index(nameof(Name), IsUnique = true)]
    public class TagEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        [Required, StringLength(32)]
        public string Name { get; private set; } = string.Empty; // 标签名称，如 "C#"、".NET 8"
        public string? AliasName { get; private set; } // 存储名词的别名

        // 导航属性：关联的中间表
        public ICollection<FileTagEntity> FileTags { get; set; } = [];

        public TagEntity() { }
        public TagEntity(string name, string? aliasName = null)
        {
            this.Name = name;
            this.AliasName = aliasName ?? name;
        }
        public void Update(string name, string? aliasName = null)
        {
            this.Name = name;
            this.AliasName = aliasName ?? name;
        }
    }
}
