using AqLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    public enum TodoStatus { Initial, Wait, Execute, Completed }

    [Table("TodoList")]
    public class TodoEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid? FTID { get; private set; } = null;// 作为父级任务ID，默认为空

        [Column, Required(ErrorMessage = "Description is required")]
        public string Desc { get; private set; } = string.Empty;

        [Column, Required(ErrorMessage = "Status is required")]
        public TodoStatus Status { get; private set; } = TodoStatus.Initial;

        [Column]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // 任务完成时间，默认为空，只有当任务状态为Completed时才会有值
        [Column]
        public DateTime? CompletedAt { get; private set; }

        // 任务优先级,数值越大优先级越高，
        // 预定于规则: 0 : 完成 ,1 - 正无穷 : 合法语义区间 , -1 : 该待办已废弃
        // 已完成的 : 不可被更新 不可被删除
        // 已废弃 :  不可被更新 可以删除
        // 合法语义区间 : 可以更新和删除
        [Column]
        public int Priority { get; private set; } = 1;

        [ForeignKey(nameof(FTID))]
        public ICollection<TodoEntity> Children { get; set; } = [];

        [NotMapped]
        public bool HasChildren => Children != null && Children.Count > 0;

        public TodoEntity(string content, int priority)
        {
            Desc = content;
            Priority = priority;
        }
        public TodoEntity() { }
        public TodoEntity(string content)
        {
            Desc = content;
        }

        public void SetParent(Guid parentId)
        {
            if (parentId == Guid.Empty)
                throw new DomainLegalityException("父任务 ID 不能为 Empty");

            if (parentId == UID)
                throw new DomainLegalityException("任务不能成为自己的父任务");

            FTID = parentId;
        }
        public void RemoveParent()
        {
            FTID = null;
        }

        public void Completed()
        {

            CompletedAt = DateTime.UtcNow;
            Priority = 0;
        }

        public void Update(string content, TodoStatus status, int priority)
        {
            if (this.Status == TodoStatus.Completed) throw new DomainLegalityException("不允许更新已完成的待办");
            Desc = content;
            Status = status;
            if (status == TodoStatus.Completed) Completed();
            else
            {
                Priority = priority;
            }
        }
    }
}
