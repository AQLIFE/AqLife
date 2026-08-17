using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Domain.Entities
{
    [Table("Corpus"),Index(nameof(CorpusContent),IsUnique =true)]
    public class CorpusEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();
        [Column]
        public string CorpusContent { get;private set; } = string.Empty;
        [Column]
        public DateTime CreateDate { get;init; } = DateTime.UtcNow;

        public CorpusEntity() { }
        public CorpusEntity(string content) { CorpusContent = content; }
        public void ChangeCorpus(string content) { CorpusContent = content; }
    }
}
