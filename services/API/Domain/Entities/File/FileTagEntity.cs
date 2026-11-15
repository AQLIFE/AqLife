using AqLife.Shared.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.File
{
    [Table("FileTags")]
    public class FileTagEntity : IRelationship
    {
        [ForeignKey(nameof(FileId))]
        public Guid FileId { get;private set; }

        [ForeignKey(nameof(TagId))]
        public Guid TagId { get;private set; }

        public virtual FileMetaEntity File { get; set; } = null!;
        public virtual TagEntity Tag { get; set; } = null!;

        public FileTagEntity(Guid fileId, Guid tagId)
        {
            FileId = fileId;
            TagId = tagId;
        }
    }
}
