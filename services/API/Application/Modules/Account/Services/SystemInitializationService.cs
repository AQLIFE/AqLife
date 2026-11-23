using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AqLife.Application.Account.Services
{
    public class SystemInitializationService(IAccountPersistence persistence, IApplicationDbContext unitOfWork)
    {
        public async Task<bool> Initialization(CancellationToken ct = default)
        {
            if (await persistence.SystemStates.AnyAsync(e => e.IsInitialized, ct))
                return false;
            if (await persistence.SystemStates.AnyAsync(ct))
                return false;

            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            string key = Convert.ToHexString(bytes);
            await persistence.AddSystemStateAsync(new SystemStateEntity(key), ct);
            await unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<string?> GetSystemKey(CancellationToken ct = default)
            => await persistence.SystemStates.Where(e => !e.IsInitialized)
                .Select(e => e.SystemKeyHash).SingleOrDefaultAsync(ct);

        public async Task<bool> ValidateSystemKey(string systemKey, CancellationToken ct = default)
        {
            SystemStateEntity? systemState = await persistence.SystemStates
                .SingleOrDefaultAsync(e => !e.IsInitialized, ct);
            return systemState is not null && systemState.SystemKeyHash == systemKey;
        }

        public async Task<bool> VoidSystemKey(string systemKey, CancellationToken ct = default)
        {
            SystemStateEntity? key = await persistence.SystemStates
                .SingleOrDefaultAsync(e => !e.IsInitialized && e.SystemKeyHash == systemKey, ct);
            if (key is null)
                return false;
            key.Initialize();
            return true;
        }
    }
}
