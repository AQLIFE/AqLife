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
            => mediator.Send(query, ct);
        
        [HttpPost]
        public Task<Guid> CreateAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPatch]
        public Task<Guid> UpdateAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
            => mediator.Send(command, ct);

        [HttpPost("{UID:guid}/archive")]
        public Task<Guid> ArchiveAsync([FromRoute(Name = nameof(ArchiveDevelopmentPlanCommand.UID))]ArchiveDevelopmentPlanCommand command, CancellationToken ct)
            => mediator.Send(command, ct);
    }
}