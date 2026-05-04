using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entity
{
    [Table("Demo")]
    public class DemoEntity
    {
        [Column, Key]
        public int Serial { get; set; } = 0;
        [Column]
        public string Desc { get; set; } = "default";
    }

    [Table("Corpus")]
    public class CorpusEntity
    {
        [Key]
        public Guid Gid { set; get; } = Guid.NewGuid();
        [Column]
        public string CorpusContent { set; get; } = "朝生暮死";
    }
}
