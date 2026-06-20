using Microsoft.EntityFrameworkCore;
using MyLife.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Data.Entities
{
    [Table("Tags"),Index(nameof(Name))]
    public class TagEntity : IEntity
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();

        [Required, StringLength(32)]
        public string Name { get; set; } = string.Empty; // 标签名称，如 "C#"、".NET 8"
        public string AliasName { get; set; } = string.Empty;// 存储名词的翻译

        public bool IsCategory { get; set; } = false; // 标记是否属于分类`

        // 导航属性：关联的中间表
        public virtual ICollection<FileTagEntity> FileTags { get; set; } = [];

        public TagEntity() { }
        public TagEntity(string name,bool isCategory =false,string? aliasName= null)
        {
            this.Name = name;
            this.AliasName = aliasName ?? name;
            this.IsCategory = isCategory;
        }
    }
}
