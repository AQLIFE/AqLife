using AqLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Entities.File
{
    public class InteractionMetaEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.Empty;

        public int ViewCount { get; private set; } = 0;
        public void Viewed() { this.ViewCount++; }

        public InteractionMetaEntity(Guid guid) { UID = guid; }
        public InteractionMetaEntity() { }
    }
}
