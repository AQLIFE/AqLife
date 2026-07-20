using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.Mappings;
using MyLife.Shared.Exceptions;
using MyLife.Shared.IView;

namespace MyLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController,Authorize]
    public class TodoController(AppStorage storage, TodoMapper mapper) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<List<TodoDto>> GetAll()
            => await storage.Todo.Include(e=>e.TodoList).Select(e => mapper.ToDto(e)).ToListAsync();

        [HttpGet("{id:guid}"), AllowAnonymous]
        public async Task<TodoDto?> GetById([FromQuery]Guid id)
            => await storage.Todo.Where(e => e.UID == id).Select(e => mapper.ToDto(e)).FirstOrDefaultAsync();

        [HttpPost]
        public async Task<string> Create(CreateTodoCommand command)
        {
            var entity = mapper.ToEntity(command);

            if (entity.FTID != null && storage.Todo.Any(e => e.UID == entity.FTID) || entity.FTID is null)
            {
                storage.Todo.Add(entity);
                await storage.SaveChangesAsync();
                return entity.UID.ToString();
            }
            throw new RequestTransactionFailedException("不存在父级任务");
        }

        [HttpPatch]
        public async Task<string> UpdateTodo(Guid guid, string status)
        {
            var todo = await storage.Todo.Where(e => e.UID == guid).FirstOrDefaultAsync();
            if (todo is TodoEntity entity)
            {
                //entity.Status = TodoMapper.ConvertStatus(status);
                entity.Status = mapper.Convert(status);
                if(entity.Status == TodoStatus.Completed)entity.CompletedAt = DateTime.Now;
                storage.Todo.Update(entity);
                await storage.SaveChangesAsync();
                return entity.Status.ToString();
            }
            throw new RequestTransactionFailedException("不存在的todo ID");

        }

        [HttpDelete]
        public async Task<bool> DeleteTodo(Guid guid)
        {
            var todo = await storage.Todo.Where(e => e.UID == guid).FirstOrDefaultAsync();
            if (todo is TodoEntity obj)
            {
                storage.Todo.Remove(obj);
                await storage.SaveChangesAsync();
            }
            throw new RequestTransactionFailedException("不存在的todo ID");
        }
    }
}
