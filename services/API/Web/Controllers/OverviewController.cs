using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class OverviewController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<OverviewDto> GetOverview([FromQuery] OverviewQuery query, CancellationToken ct)
            => await mediator.Send(query, ct);
    }
}
