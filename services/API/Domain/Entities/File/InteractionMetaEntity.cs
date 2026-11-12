using AqLife.Shared.Contracts;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Entities.File
{
    public class InteractionMetaEntity : IRelationship
    {
        [Key]
        public Guid UID { get; init; } = Guid.Empty;// UID 也作为外键,导向 FileMeatEntity Id

        public int ViewCount { get; private set; } = 0;
        public void Viewed() { this.ViewCount++; }

        public InteractionMetaEntity(Guid guid) { UID = guid; }
        public InteractionMetaEntity() { }
    }
}
