using Microsoft.Extensions.Diagnostics.HealthChecks;
using MyLife.Data.Health;
using MyLife.Shared.Options;
using Serilog;

namespace MyLife.Web.Infrastructure
{
    public class DatabaseSmokeTest(HealthFunc statusFunc) : IHealthCheck
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
                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ApiType, "API 逻辑检查...");
                var obj = status switch
                {
                    APIStatus { Write: true, Read: true, Update: true, Delete: true, } => HealthCheckResult.Healthy("冒烟测试完全通过"),
                    APIStatus { Write: true, Read: false } => HealthCheckResult.Unhealthy("读取测试失败"),
                    APIStatus { Write: false } => HealthCheckResult.Unhealthy("写入测试失败"),
                    _ => HealthCheckResult.Unhealthy("冒烟测试失败")
                };


                Log.Information(@"[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ApiType, obj.Description?.ToString());
                return obj;

            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("系统写入崩溃", ex);
            }
        }
    }
}
