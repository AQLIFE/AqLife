using System.ComponentModel.DataAnnotations;

namespace MyLife.Shared.DTOs
{
    public record TodoDto
    (
        Guid? FTID,
        [Required(ErrorMessage = "Description is required")]
        string Desc,
        [Required(ErrorMessage = "Status is required")]
        string Status,
        DateTime CreatedAt,

        DateTime? CompletedAt,// 任务完成时间，默认为空，只有当任务状态为Completed时才会有值

        int Priority// 任务优先级,数值越大优先级越高，默认为0
    ) : IEntityDto;


    public record TodoForAdd
    (
        Guid? FTID,
        [Required(ErrorMessage = "Description is required")]
        string Desc,

        int Priority// 任务优先级,数值越大优先级越高，默认为0
    ) : IEntityDto;
}
