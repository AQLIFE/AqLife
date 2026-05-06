using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;

namespace MyLife.Data.Repository
{
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options)
    {
        public DbSet<DemoEntity> Demo { get; set; }
        public DbSet<FileIndexEntity> File { get; set; }
        public DbSet<CorpusEntity> Corpus { get; set; }
        public DbSet<AccountEntity> Account { get; set; }
        public DbSet<SubscriptionEntity> Subscription { get; set; }
    }
}
