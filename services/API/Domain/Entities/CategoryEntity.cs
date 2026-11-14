using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("Category")]
    [Index(nameof(CategoryName), IsUnique = true)]
    public class CategoryEntity : IEntity
    {
        public static readonly Guid DefaultCategoryId = new("3B246A0E-ED94-45E0-930D-15643683D50D");
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

        [MapperConstructor]
        public CategoryEntity(string categoryName, string categoryDescription, Guid? parentId = null):this(categoryName)
        {
            CategoryDescription = categoryDescription;
            ParentId = parentId;
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
