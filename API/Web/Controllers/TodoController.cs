using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Service.Mappings;
using MyLife.Shared.Accident;
using MyLife.Shared.DTOs;

namespace MyLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoController(AppStorage storage, TodoMapper mapper) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<List<TodoDto>> GetAll()
            => await storage.Todo.Select(e => mapper.Desensitization(e)).ToListAsync();

        [HttpGet("{id:guid}"), AllowAnonymous]
        public async Task<TodoDto?> GetById(Guid id)
            =>await storage.Todo.Where(e=>e.TID==id).Select(e=>mapper.Desensitization(e)).FirstOrDefaultAsync();

        [HttpPost, Authorize]
        public async Task<string> Create(TodoForAdd todo)
        {
            var entity = mapper.Assembly(todo);

            if (entity.FTID != null && storage.Todo.Any(e => e.TID == entity.FTID) || entity.FTID is null) 
            {
                storage.Todo.Add(entity);
                await storage.SaveChangesAsync();
                return entity.TID.ToString();
            }
            throw new OperateTransactionFailedException("不存在父级任务");
        }

        [HttpPatch, Authorize]
        public async Task<string> UpdateTodo(Guid guid, string status)
        {
            var todo = await storage.Todo.Where(e => e.TID == guid).FirstOrDefaultAsync();
            if (todo is TodoEntity entity)
            {
                entity.Status = TodoMapper.ConvertStatus(status);
                storage.Todo.Update(entity);
                await storage.SaveChangesAsync();
                return entity.Status.ToString();
            }
            throw new OperateTransactionFailedException("不存在的todo ID");

        }

        [HttpDelete, Authorize]
        public async Task<bool> DeleteTodo(Guid guid)
        {
            var todo = await storage.Todo.Where(e => e.TID == guid).FirstOrDefaultAsync();
            if (todo is TodoEntity obj)
            {
                storage.Todo.Remove(obj);
                await storage.SaveChangesAsync();
            }
            throw new OperateTransactionFailedException("不存在的todo ID");
        }
    }
}
