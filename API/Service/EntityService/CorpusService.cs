using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Entities;

namespace MyLife.Service.EntityService
{
    public class CorpusService(AppStorage storage)
    {
        public async Task<IEnumerable<CorpusEntity?>> SearchAsync(string keyword)
            => await storage.Corpus.AsNoTracking().Where(e => e.CorpusContent.Contains(keyword ?? string.Empty)).ToListAsync();
        public async Task<CorpusEntity?> GetByIdAsync(Guid id)
            => await storage.Corpus.AsNoTracking().FirstOrDefaultAsync(e => e.UID == id);
        public async Task<IEnumerable<CorpusEntity>> GetAllAsync()
            => await storage.Corpus.AsNoTracking().ToListAsync();

    }
}
