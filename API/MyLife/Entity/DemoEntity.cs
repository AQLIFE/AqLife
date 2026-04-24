using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Entity
{
    [Table("Demo")]
    public class DemoEntity
    {
        [Column, Key]
        public int Serial { get; set; } = 0;
        [Column]
        public string Desc { get; set; } = "default";
    }
}
