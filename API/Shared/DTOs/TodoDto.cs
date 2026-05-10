using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.DTOs
{
    public class TodoDto
    {
        public Guid? FTID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public required string Desc {  get; set; }
        [Required(ErrorMessage = "Status is required")]
        public required string Status {  get; set; }
        public DateTime CreatedAt { get; set; }
        // 任务完成时间，默认为空，只有当任务状态为Completed时才会有值
        public DateTime? CompletedAt { get; set; }
        // 任务优先级,数值越大优先级越高，默认为0
        public int Priority { get; set; }
    }


    public class TodoForAdd
    {
        public Guid? FTID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public required string Desc { get; set; }
        // 任务优先级,数值越大优先级越高，默认为0
        public int Priority { get; set; }
    }
}
