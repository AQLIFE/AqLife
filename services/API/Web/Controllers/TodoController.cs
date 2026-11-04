using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController, Authorize]
    public class TodoController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public Task<PageResult<TodoDto>> SearchAsync([FromQuery] TodoQuery query, CancellationToken ct)
            => mediator.Send(query, ct);

        [HttpGet("roadmap")]
        public Task<TodoRoadmapDto> GetRoadmapAsync([FromQuery] TodoRoadmapQuery query, CancellationToken ct)
            => mediator.Send(query, ct);

        [HttpPost]
        public Task<Guid> CreateAsync(CreateTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch]
        public Task<Guid> UpdateAsync(UpdateTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("parent")]
        public Task<Guid> SetParentAsync(SetTodoParentCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("sort-order")]
        public Task<Guid> SetSortOrderAsync(SetTodoSortOrderCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPost("{uid:guid}/archive")]
        public Task<Guid> ArchiveAsync(Guid uid, CancellationToken ct)
            => mediator.Send(new ArchiveTodoCommand(uid), ct);

        [HttpPost("{uid:guid}/restore")]
        public Task<Guid> RestoreAsync(Guid uid, CancellationToken ct)
            => mediator.Send(new RestoreTodoCommand(uid), ct);

        [HttpPost("dependencies")]
        public Task<Guid> AddDependencyAsync(AddTodoDependencyCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpDelete("dependencies")]
        public Task<Guid> RemoveDependencyAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPut("roadmap/layout")]
        public Task<Guid> SaveLayoutAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpDelete]
        public Task DeleteAsync(DeleteTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);
    }
}