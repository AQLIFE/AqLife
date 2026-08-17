using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController, Authorize]
    public class TodoController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<TodoDto>?> GetAllAsync(CancellationToken ct)
            => await mediator.Send(new TodoQuery(), ct);

        [HttpGet("search"), AllowAnonymous]
        public async Task<IEnumerable<TodoDto>?> SearchAsync([FromQuery] TodoQuery query, CancellationToken ct)
            => await mediator.Send(query, ct);

        [HttpPost]
        public async Task<Guid> CreateAsync(CreateTodoCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpPatch]
        public async Task<Guid> UpdateTodoStatusAsync(UpdateTodoCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteTodoAsync(DeleteTodoCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);
    }
}
