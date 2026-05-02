using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyLife.Core.Provider;
using MyLife.Entity;
using Serilog;

namespace MyLife.Core.Func
{

    public struct APIStatus
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }

    }
    public class StatusFunc(AppStorage storage)
    {


        public async Task<List<DemoEntity>?> GetAll()
        => await storage.Demo.AsNoTracking().ToListAsync();

        public async Task<DemoEntity?> Get(int serial)
        => await storage.Demo.AsNoTracking().Where(e => e.Serial == serial).FirstOrDefaultAsync();

        public async Task<DemoEntity?> Add()
        {
            var searial = await storage.Demo.CountAsync();
            var obj = new Entity.DemoEntity { Serial = searial + 1, Desc = $"测试数据{searial + 1}" };
            await storage.Demo.AddAsync(obj);
            await storage.SaveChangesAsync();
            return await storage.Demo.Where(e => e.Desc == obj.Desc).FirstOrDefaultAsync();
        }

        public async Task<DemoEntity?> Update(int serial)
        {
            var obj = await storage.Demo.Where(e => e.Serial == serial).FirstOrDefaultAsync();

            if (obj is DemoEntity demo)
            {
                demo.Desc = $"测试数据{serial} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
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

    public class DatabaseSmokeTest(StatusFunc statusFunc) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
        {
            try
            {
                var status = new APIStatus
                {
                    Write = await statusFunc.Add() != null,
                    Read = await statusFunc.GetAll() != null,
                    Update = await statusFunc.Update() != null,
                    Delete = await statusFunc.Delete() != 0
                };
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, "API 逻辑检查...");
                var obj = status switch
                {
                    APIStatus { Write: true, Read: true, Update: true, Delete: true, } => HealthCheckResult.Healthy("冒烟测试完全通过"),
                    APIStatus { Write: true, Read: false } => HealthCheckResult.Unhealthy("读取测试失败"),
                    APIStatus { Write: false } => HealthCheckResult.Unhealthy("写入测试失败"),
                    _ => HealthCheckResult.Unhealthy("冒烟测试失败")
                };

                
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, obj.Description?.ToString());
                return obj;

            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("系统写入崩溃", ex);
            }
        }

    }
}
