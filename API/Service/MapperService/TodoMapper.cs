
using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class TodoMapper : IGenMapper<TodoEntity, TodoDto>
    {
        [MapperIgnoreSource(nameof(TodoEntity.Parent))]
        [MapperIgnoreSource(nameof(TodoEntity.UID))]
        public partial TodoDto ToDto(TodoEntity entity);

        [MapperIgnoreTarget(nameof(TodoEntity.Parent))]
        [MapperIgnoreTarget(nameof(TodoEntity.UID))]
        public partial TodoEntity ToEntity(TodoDto dto);

        [MapperIgnoreTarget(nameof(TodoEntity.UID))]
        [MapperIgnoreTarget(nameof(TodoEntity.Parent))]
        public partial void UpdateEntity(TodoDto dto, TodoEntity entity);

        private TodoStatus Convert(string status)
        => status switch
        {
            "Initial" => TodoStatus.Initial,
            "Wait" => TodoStatus.Wait,
            "Execute" => TodoStatus.Execute,
            "Completed" => TodoStatus.Completed,
            _ => throw new ArgumentException("无效的待办状态", nameof(status))
        };

        private string Convert(TodoStatus status)
           => status.ToString();

        [MapperIgnoreTarget(nameof(TodoEntity.Status))]
        [MapperIgnoreTarget(nameof(TodoEntity.Parent))]
        [MapperIgnoreTarget(nameof(TodoEntity.CompletedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.UID))]
        public partial TodoEntity Assembly(TodoForAdd todo);
        // 不使用前端ID,后端自生成
    }
}
