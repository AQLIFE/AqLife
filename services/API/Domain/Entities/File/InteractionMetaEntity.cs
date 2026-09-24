using AqLife.Domain.Contracts;
using AqLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Domain.Entities.File
{
    public class InteractionMetaEntity: IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.Empty;
        [Column]
        public int ViewCount { get; private set; } = 0;
        //[Column]
        //public int LikeCount { get; private set; } = 0;
        //[Column]
        //public int ShareCount { get; private set; } = 0;

        public void Viewed() { this.ViewCount++; }

        public InteractionMetaEntity(Guid guid) { UID = guid; }
        public InteractionMetaEntity() { }
    }
}
