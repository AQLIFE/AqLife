using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Core.API;
using MyLife.Core.Define;
using MyLife.Core.Provider;
using MyLife.Entity;

namespace MyLife.Controllers
{
    [ApiController,Route("system")]
    public class SystemController(AppStorage storage): ControllerBase
    {
        [HttpGet("status")]
        public async Task<DemoEntity?> Status()
        {
            var searial =await  storage.Demo.CountAsync();
            var obj = new Entity.DemoEntity {Serial = searial+1, Desc = $"测试数据{searial+1}" };
            await storage.Demo.AddAsync(obj);
            await storage.SaveChangesAsync();
            return await storage.Demo.Where(e => e.Desc == obj.Desc).FirstOrDefaultAsync();
        }
    }
}