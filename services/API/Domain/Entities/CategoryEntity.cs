using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("Category")]
    [Index(nameof(CategoryName), IsUnique = true)]
    public class CategoryEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();
        [Required, StringLength(32, ErrorMessage = "分类名称长度必须在0-32个字符之间")]
        public string CategoryName { get; private set; } = String.Empty;
        [StringLength(256, ErrorMessage = "分类描述长度必须在0-256个字符之间")]
        public string CategoryDescription { get; private set; } = String.Empty;
        public Guid? ParentId { get; private set; } = null;

        [ForeignKey(nameof(ParentId))]
        public CategoryEntity? ParentCategory { get; private set; } = null;

        public virtual ICollection<FileMetaEntity> Files { get; set; } = [];
        public virtual ICollection<FavoriteItemEntity> Favorites { get; set; } = [];


        public CategoryEntity() { }
        public CategoryEntity(string categoryName)
        {
            CategoryName = categoryName;
        }

        public CategoryEntity(string categoryName, string categoryDescription, CategoryEntity? parentCategory = null)
        {
            CategoryName = categoryName;
            CategoryDescription = categoryDescription;
            ParentCategory = parentCategory;
            ParentId = parentCategory?.UID;
        }

        public void SetParentCategory(CategoryEntity? parentCategory)
        {
            this.ParentCategory = parentCategory;
            this.ParentId = parentCategory?.UID;
        }

        public void Update(string categoryName, string categoryDescription)
        {
            CategoryName = categoryName;
            CategoryDescription = categoryDescription;
        }
    }
}
