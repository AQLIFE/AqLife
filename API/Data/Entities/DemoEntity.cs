using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entities
{
    [Table("Demo")]
    public class DemoEntity
    {
        [Column, Key]
        public int Serial { get; set; } = 0;
        [Column]
        public string Desc { get; set; } = string.Empty;
    }

    [Table("Corpus")]
    public class CorpusEntity
    {
        [Key]
        public Guid Gid { set; get; } = Guid.NewGuid();
        [Column]
        public string CorpusContent { set; get; } = string.Empty;
    }
}
