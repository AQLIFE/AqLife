using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{
    [Route("development-plans")]
    [ApiController, Authorize]
    public class DevelopmentPlanController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public Task<IReadOnlyList<DevelopmentPlanDto>> GetAsync([FromQuery] DevelopmentPlanQuery query, CancellationToken ct)
            => mediator.Send(query, ct);

        [HttpPost]
        public Task<Guid> CreateAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch]
        public Task<Guid> UpdateAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPost("{uid:guid}/archive")]
        public Task<Guid> ArchiveAsync(Guid uid, CancellationToken ct)
            => mediator.Send(new ArchiveDevelopmentPlanCommand(uid), ct);
    }
}