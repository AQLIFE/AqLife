using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController,Authorize]
    public class TagController(IMediator mediator) : ControllerBase
    {
        [HttpGet,AllowAnonymous]
        public async Task<IEnumerable<TagDto>?> Search([FromQuery]TagQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);
        [HttpPost]
        public async Task<Guid> AddTag(CreateTagCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpPatch]
        public async Task<Guid> UpdateTag(UpdateTagCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteTag(DeleteTagCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);
    }
}
