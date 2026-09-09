using AqLife.Domain.Contracts;
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
        public string Name { get; set; } = string.Empty; // 标签名称，如 "C#"、".NET 8"
        public string? AliasName { get; set; } // 存储名词的别名

        public bool IsCategory { get; set; } = false; // 标记是否属于分类

        // 导航属性：关联的中间表
        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public TagEntity() { }
        public TagEntity(string name, bool isCategory = false, string? aliasName = null)
        {
            this.Name = name;
            this.AliasName = aliasName ?? name;
            this.IsCategory = isCategory;
        }
        public void Update(string name, bool isCategory = false, string? aliasName = null)
        {
            this.Name = name;
            this.AliasName = aliasName ?? name;
            this.IsCategory = isCategory;
        }

        public void SetCategory()
        {
            IsCategory = true;
        }
        public void CancelCategory()
        {
            IsCategory = false;
        }
    }
}
