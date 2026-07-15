using MyLife.Shared.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entities
{
    [Table("Corpus")]
    public class CorpusEntity : IEntity
    {
        [Key]
        public Guid UID { set; get; } = Guid.NewGuid();
        [Column]
        public string CorpusContent { set; get; } = string.Empty;
    }
}
