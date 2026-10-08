using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Account.Services
{
    public class SystemInitializationService(IApplicationDbContext context)
    {
        public async Task<bool> Initialization(CancellationToken ct = default)
        {
            if (await context.SystemStates.AnyAsync(e => e.IsInitialized, ct))
                return false;

            if (await context.SystemStates.AnyAsync(ct))
                return false;

            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            string key = Convert.ToHexString(bytes);

            await context.SystemStates.AddAsync(
                new SystemStateEntity(key),
                ct);

            await context.SaveChangesAsync(ct);

            return true;
        }

        public async Task<string?> GetSystemKey(CancellationToken ct = default)
        {
            return await context.SystemStates
                .Where(e => !e.IsInitialized)
                .Select(e => e.SystemKeyHash)
                .SingleOrDefaultAsync(ct);
        }

        public async Task<bool> ValidateSystemKey(string systemKey,CancellationToken ct = default)
        {
            SystemStateEntity? systemState =
                await context.SystemStates
                    .SingleOrDefaultAsync(
                        e => !e.IsInitialized,
                        ct);

            if (systemState is null)
                return false;

            return systemState.SystemKeyHash == systemKey;
        }

        public async Task<bool> VoidSystemKey(string systemKey,CancellationToken ct = default)
        {
            SystemStateEntity? key =
                await context.SystemStates
                    .SingleOrDefaultAsync(
                        e => !e.IsInitialized &&
                             e.SystemKeyHash == systemKey,
                        ct);

            if (key is null)
                return false;

            key.Initialize();

            return true;
        }
    }
}
