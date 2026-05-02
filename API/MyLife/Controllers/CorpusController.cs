using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Core.Provider;
using MyLife.Entity;

namespace MyLife.Controllers
{
    [Route("Corpus"), ApiController]
    public class CorpusController(AppStorage storage) : ControllerBase
    {
        [HttpGet]
        public async Task<List<string>> GetAllCorpus()
            => await storage.Corpus.AsNoTracking().Select(e => e.CorpusContent).ToListAsync();

        [HttpGet("{guid:guid}")]
        public async Task<string> GetCorpus([FromRoute] Guid guid)        
            => await storage.Corpus.AsNoTracking().Where(e => e.Gid == guid).Select(e=>e.CorpusContent).FirstOrDefaultAsync() ?? "不存在语料";

        

        [HttpGet("Random")]
        public async Task<string> GetRandomCorpus()
        {
            var count = await storage.Corpus.CountAsync();
            if (count == 0) return "没有任何语料";
            var randomIndex = new Random().Next(count);
            var corpus = await storage.Corpus.Skip(randomIndex).FirstOrDefaultAsync();
            return corpus?.CorpusContent ?? "不存在语料";
        }

        [HttpPost]
        public async Task<string> AddCorpus(string content)
        {
            var corpus = new CorpusEntity { CorpusContent = content };
            await storage.Corpus.AddAsync(corpus);
            await storage.SaveChangesAsync();
            return corpus.Gid.ToString();
        }

        [HttpDelete]
        public async Task<int> DeleteCorpus(Guid guid)
        {
            var corpus = await storage.Corpus.FirstOrDefaultAsync(e => e.Gid == guid);
            if (corpus == null) return 0;
            storage.Corpus.Remove(corpus);
            return await storage.SaveChangesAsync();            
        }
    }
}
