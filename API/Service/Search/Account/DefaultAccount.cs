using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Search.Account
{
    public class DefaultAccount : IAccountSearchStrategy
    {
        public async Task<IEnumerable<AccountEntity>> ExecuteAsync(IQueryable<AccountEntity> queryable, Guid? UID = null)
        =>await queryable.Where(e => e.IsValid).ToListAsync();
        public bool IsMatch(Guid? UID = null) => UID == null;
    }
}
