using MyLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Domain.Entities
{
    public enum TodoStatus { Initial, Wait, Execute, Completed }

    [Table("TodoList")]
    public class TodoEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid? FTID { get; set; } = null;// 作为父级任务ID，默认为空

        [Column, Required(ErrorMessage = "Description is required")]
        public string Desc { get; set; } = string.Empty;

        [Column, Required(ErrorMessage = "Status is required")]
        public TodoStatus Status { get; set; } = TodoStatus.Initial;

        [Column]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 任务完成时间，默认为空，只有当任务状态为Completed时才会有值
        [Column]
        public DateTime? CompletedAt { get; set; }
        [Column]
        // 任务优先级,数值越大优先级越高，默认为0
        public int Priority { get; set; } = 0;

        [ForeignKey(nameof(FTID))]
        public ICollection<TodoEntity> Children { get; set; } = [];

        [NotMapped]
        public bool HasChildren => Children != null && Children.Count > 0;


        public void Update(string content, TodoStatus status, int priority=0)
        {
            Desc = content;
            Status = status;
            if (status == TodoStatus.Completed)
            {
                CompletedAt = DateTime.UtcNow;
                priority = 0;
            }
            Priority = priority;
        }
    }
}
