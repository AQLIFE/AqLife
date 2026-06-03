using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;

namespace MyLife.Web.Controllers
{
    [Route("[controller]"), ApiController]
    public class CorpusController(AppStorage storage) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<List<string>> GetAllCorpus()
            => await storage.Corpus.AsNoTracking().Select(e => e.CorpusContent).ToListAsync();


        [HttpGet("search"), AllowAnonymous]
        public async Task<string> FuzzeSearch(string query)
            => await storage.Corpus.AsNoTracking().Where(e => e.CorpusContent.Contains(query)).Select(e => e.CorpusContent).FirstOrDefaultAsync() ?? "不存在语料";

        [HttpGet("{guid:guid}")]
        public async Task<string> GetCorpus([FromRoute] Guid guid)
            => await storage.Corpus.AsNoTracking().Where(e => e.UID == guid).Select(e => e.CorpusContent).FirstOrDefaultAsync() ?? "不存在语料";

        [HttpGet("random"), AllowAnonymous]
        public async Task<string> GetRandomCorpus()
        {
            var count = await storage.Corpus.CountAsync();
            if (count == 0) return "没有任何语料";
            var randomIndex = new Random().Next(count);
            var corpus = await storage.Corpus.OrderBy(e => e.UID).Skip(randomIndex).FirstOrDefaultAsync();
            return corpus?.CorpusContent ?? "不存在语料";
        }

        [HttpPost, Authorize]
        public async Task<string> AddCorpus(string content)
        {
            var corpus = new CorpusEntity { CorpusContent = content };
            await storage.Corpus.AddAsync(corpus);
            await storage.SaveChangesAsync();
            return corpus.UID.ToString();
        }

        [HttpDelete, Authorize]
        public async Task<int> DeleteCorpus(Guid guid)
        {
            var corpus = await storage.Corpus.FirstOrDefaultAsync(e => e.UID == guid);
            if (corpus == null) return 0;
            storage.Corpus.Remove(corpus);
            return await storage.SaveChangesAsync();
        }
    }
}
