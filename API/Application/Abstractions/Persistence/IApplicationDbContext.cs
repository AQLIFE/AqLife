using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Entities;

namespace MyLife.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        public DbSet<FileMetaEntity> File { get;  }
        public DbSet<CorpusEntity> Corpus { get;  }
        public DbSet<AccountEntity> Accounts { get;  }
        public DbSet<SubscriptionEntity> Subscription { get;  }
        public DbSet<TodoEntity> Todo { get;  }
        public DbSet<TagEntity> Tags { get;  }
        public DbSet<FileTagEntity> BlogTags { get;  }


        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
        Task<ITransaction> BeginTransactionAsync(
        CancellationToken ct);

        DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    }
}
