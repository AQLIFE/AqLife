using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers;

[ApiController, Route("[controller]"), Authorize]
public sealed class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<PageResult<CategoryDto>?> Search([FromQuery] CategoryQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);

    [HttpPost]
    public async Task<Guid> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

    [HttpPut]
    public async Task<Guid> Update([FromBody] UpdateCategoryCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

    [HttpDelete]
    public async Task Delete([FromQuery] DeleteCategoryCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);
}
