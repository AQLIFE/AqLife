using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Web.Controllers
{
    [Route("[controller]"), ApiController, Authorize]
    public class CorpusController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CorpusDto>?> AllAsync(CancellationToken ct)
            => await mediator.Send(new CorpusQuery(), ct);


        [HttpGet("search")]
        public async Task<IEnumerable<CorpusDto>?> SearchAsync([FromQuery] CorpusQuery query, CancellationToken ct)
            => await mediator.Send(query, ct);

        [HttpGet("random"), AllowAnonymous]
        public async Task<string> GetRandomCorpus(CancellationToken ct)
        => await mediator.Send(new RandomCorpusQuery(), ct);
        
        [HttpPost]
        public async Task<Guid> AddCorpus(CreateCorpusCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteCorpus(DeleteCorepusCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);
    }
}
