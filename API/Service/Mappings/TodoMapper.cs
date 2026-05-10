
using MyLife.Data.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class TodoMapper: IGenericsMapper<TodoEntity, TodoDto>
    {
        [MapperIgnoreSource(nameof(TodoEntity.Parent))]
        [MapperIgnoreSource(nameof(TodoEntity.TID))]
        public partial TodoDto Desensitization(TodoEntity entity);

        [MapperIgnoreTarget(nameof(TodoEntity.Parent))]
        [MapperIgnoreTarget(nameof(TodoEntity.TID))]
        public partial TodoEntity Assembly(TodoDto dto);

        public static TodoStatus ConvertStatus(string status)
        => status switch
        {
            "Initial" => TodoStatus.Initial,
            "Wait" => TodoStatus.Wait,
            "Execute" => TodoStatus.Execute,
            "Completed" => TodoStatus.Completed,
            _ => throw new ArgumentException("无效的待办状态", nameof(status))
        };

        public static string ConvertStatus(TodoStatus status)
           => status.ToString();

        [MapperIgnoreTarget(nameof(TodoEntity.Status))]
        [MapperIgnoreTarget(nameof(TodoEntity.Parent))]
        [MapperIgnoreTarget(nameof(TodoEntity.CompletedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.TID))]
        public partial TodoEntity Assembly(TodoForAdd todo);
        // 不使用前端ID,后端自生成
    }
}
