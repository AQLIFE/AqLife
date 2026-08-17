using MyLife.Application.Abstractions.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Business.Todo
{
    [Mapper]
    public partial class TodoMapper : IViewMapper<TodoEntity, TodoDto>, ICreateMapper<TodoEntity, CreateTodoCommand>
    {
        [MapProperty(nameof(TodoEntity.Children),nameof(TodoDto.TodoList))]
        public partial TodoDto ToDto(TodoEntity entity);

        [MapperIgnoreTarget(nameof(TodoEntity.UID))]
        [MapperIgnoreTarget(nameof(TodoEntity.Status))]
        [MapperIgnoreTarget(nameof(TodoEntity.CompletedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.Children))]
        public partial TodoEntity ToEntity(CreateTodoCommand command);

        //public TodoStatus Convert(string status)
        //=> status switch
        //{
        //    "Initial" => TodoStatus.Initial,
        //    "Wait" => TodoStatus.Wait,
        //    "Execute" => TodoStatus.Execute,
        //    "Completed" => TodoStatus.Completed,
        //    _ => throw new ArgumentException("无效的待办状态", nameof(status))
        //};

        public string Convert(TodoStatus status)
           => status.ToString();
        // 不使用前端ID,后端自生成
    }
}
