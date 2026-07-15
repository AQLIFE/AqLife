using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;

namespace MyLife.Data.Repository
{
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options)
    {
        //public DbSet<DemoEntity> Demo { get; set; }
        public DbSet<FileMetaEntity> File { get; set; }
        public DbSet<CorpusEntity> Corpus { get; set; }
        public DbSet<AccountEntity> Account { get; set; }
        public DbSet<SubscriptionEntity> Subscription { get; set; }
        public DbSet<TodoEntity> Todo { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<FileTagEntity> BlogTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 为中间表配置复合主键 [cite: 5, 158]
            modelBuilder.Entity<FileTagEntity>()
                .HasKey(ft => new { ft.FileId, ft.TagId });

            // 显式声明关系（可选，但推荐以增强健壮性）
            modelBuilder.Entity<FileTagEntity>()
            .HasOne(ft => ft.File)
            .WithMany(f => f.FileTags)
            .HasForeignKey(ft => ft.FileId)
            .OnDelete(DeleteBehavior.Cascade); // 关键：开启级联删除

            // 3. 配置 TagEntity -> FileTagEntity 的关系及级联删除
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FileTags)
                .HasForeignKey(ft => ft.TagId)
                .OnDelete(DeleteBehavior.Cascade); // 关键：开启级联删除
        }
    }
}
