using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers;

[ApiController, Route("[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<PageResult<CategoryDto>?> Search([FromQuery] CategoryQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);
}
