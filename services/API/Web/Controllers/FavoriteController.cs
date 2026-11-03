using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers;

[ApiController, Route("[controller]")]
public class FavoriteController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<PageResult<FavoriteDto>> Search([FromQuery] FavoriteQuery query, CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
            query = query with { IncludeUnpublished = false };
        return await mediator.Send(query, ct);
    }

    [HttpPost, Authorize]
    public async Task<Guid> Create([FromBody] CreateFavoriteCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

    [HttpPut, Authorize]
    public async Task<Guid> Update([FromBody] UpdateFavoriteCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

    [HttpPatch("publish"), Authorize]
    public async Task<Guid> SetPublished([FromBody] SetFavoritePublishedCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

    [HttpDelete, Authorize]
    public async Task Delete([FromQuery] DeleteFavoriteCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);
}
