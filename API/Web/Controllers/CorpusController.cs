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
        public async Task<CorpusDto?> GetRandomCorpus(CancellationToken ct)
        => await mediator.Send(new RandomCorpusQuery(), ct);
        //{
        //    var count = await storage.Corpus.CountAsync();
        //    if (count == 0) return "没有任何语料";
        //    var randomIndex = new Random().Next(count);
        //    var corpus = await storage.Corpus.OrderBy(e => e.UID).Skip(randomIndex).FirstOrDefaultAsync();
        //    return corpus?.CorpusContent ?? "不存在语料";
        //}

        [HttpPost]
        public async Task<Guid> AddCorpus(CreateCorpusCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteCorpus(DeleteCorepusCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);
    }
}
