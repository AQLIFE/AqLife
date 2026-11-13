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

        [HttpPost]
        public Task<Guid> CreateAsync(CreateTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("{uid:guid}/details")]
        public Task<Guid> UpdateAsync([FromRoute(Name = nameof(UpdateTodoCommand.UID))] UpdateTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("{uid:guid}/status")]
        public Task<Guid> SetParentAsync([FromRoute(Name = nameof(UpdateTodoStatusCommand.UID))] UpdateTodoStatusCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("{uid:guid}/archive")]
        public Task<Guid> ArchiveAsync([FromRoute(Name = nameof(ArchiveTodoCommand.UID))] ArchiveTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch("{uid:guid}/restore")]
        public Task<Guid> RestoreAsync([FromRoute(Name = nameof(RestoreTodoCommand.UID))]RestoreTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);
        [HttpDelete]
        public Task DeleteAsync(DeleteTodoCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPost("dependencies")]
        public Task<Guid> AddDependencyAsync(AddTodoDependencyCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpDelete("dependencies")]
        public Task RemoveDependencyAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
            => mediator.Send(command, ct);
        
        [HttpGet("roadmap"), AllowAnonymous]
        public Task<TodoRoadmapDto> GetRoadmapAsync([FromQuery] TodoRoadmapQuery query, CancellationToken ct)
            => mediator.Send(query, ct);

        [HttpPut("roadmap/layout")]
        public Task<Guid> SaveLayoutAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

    }
}