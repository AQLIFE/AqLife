using MyLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Domain.Entities
{
    [Table("Corpus")]
    public class CorpusEntity : IEntity
    {
        [Key]
        public Guid UID { set; get; } = Guid.NewGuid();
        [Column]
        public string CorpusContent { set; get; } = string.Empty;

        public CorpusEntity() { }
        public CorpusEntity(string content) { CorpusContent = content; }
    }
}
