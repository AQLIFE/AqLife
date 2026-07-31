using MyLife.Domain.CommandInterface;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace MyLife.Domain.Command
{
    public record TodoQuery(Guid? UID = null, string? Desc = null,bool IsTree=true) : IQuery<IEnumerable<TodoDto>?>;
    public record CreateTodoCommand([Required(ErrorMessage = "Description is required")] string Desc, [Range(minimum: 1, maximum: 99, ErrorMessage = "新建待办不允许为无效优先级")] int Priority = 1, Guid? FTID = null) : ICreateCommand;

    public record UpdateTodoCommand(Guid UID, [Required(ErrorMessage = "Description is required")] string Desc, [property: JsonConverter(typeof(JsonStringEnumConverter))] TodoStatus Status, [Range(minimum: 1, maximum: 99, ErrorMessage = "新建待办不允许为无效优先级")] int Priority) : IRequireValidEntity<TodoEntity>, IUpdateCommand;

    public record DeleteTodoCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<TodoEntity>;


}
