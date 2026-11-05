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
        [HttpGet, AllowAnonymous]
        public Task<IReadOnlyList<DevelopmentPlanDto>> GetAsync([FromQuery] DevelopmentPlanQuery query, CancellationToken ct)
        {
            // Anonymous Web visitors can only discover active, public plans.
            var effectiveQuery = User.Identity?.IsAuthenticated == true
                ? query
                : query with { IncludeArchived = false };
            return mediator.Send(effectiveQuery, ct);
        }

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