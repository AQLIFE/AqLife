using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;

namespace MyLife.Data.Repository
{
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options)
    {
        public DbSet<DemoEntity> Demo { get; set; }
        public DbSet<FileIndexEntity> File { get; set; }
        public DbSet<CorpusEntity> Corpus { set; get; }
    }
}
