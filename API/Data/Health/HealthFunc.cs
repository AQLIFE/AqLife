using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;

namespace MyLife.Data.Health
{
    public class HealthFunc(AppStorage storage)
    {
        public async Task<List<DemoEntity>?> GetAll()
        => await storage.Demo.AsNoTracking().ToListAsync();

        public async Task<DemoEntity?> Get(int serial)
        => await storage.Demo.AsNoTracking().Where(e => e.Serial == serial).FirstOrDefaultAsync();

        public async Task<DemoEntity?> Add()
        {
            var searial = await storage.Demo.CountAsync();
            var obj = new DemoEntity { Serial = searial + 1, Desc = $"测试数据{searial + 1}" };
            await storage.Demo.AddAsync(obj);
            await storage.SaveChangesAsync();
            return await storage.Demo.Where(e => e.Desc == obj.Desc).FirstOrDefaultAsync();
        }

        public async Task<DemoEntity?> Update(int serial)
        {
            var obj = await storage.Demo.Where(e => e.Serial == serial).FirstOrDefaultAsync();

            if (obj is DemoEntity demo)
            {
                demo.Desc = $"测试数据{serial} - {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}";
                await storage.SaveChangesAsync();
                return demo;
            }
            return null;
        }

        public async Task<DemoEntity?> Update()
            => await Update((await GetAll())?.Count ?? 0);


        public async Task<int> Delete(int serial)
        {
            var obj = await storage.Demo.Where(e => e.Serial == serial).FirstOrDefaultAsync();
            if (obj is DemoEntity demo)
            {
                storage.Demo.Remove(demo);
                return await storage.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> Delete()
        => await Delete((await GetAll())?.Count ?? 0);
    }
}
