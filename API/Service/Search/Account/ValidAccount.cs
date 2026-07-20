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
    public class ValidAccount : IAccountSearchStrategy
    {
        public bool IsMatch(Guid? UID) => UID is Guid;
        public async Task<IEnumerable<AccountEntity>> ExecuteAsync(
            IQueryable<AccountEntity> queryable,
            Guid? UID = null)
        => await queryable.Where(e => e.UID == UID).ToListAsync();
    }
}
