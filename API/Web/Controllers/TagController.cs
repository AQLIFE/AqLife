using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyLife.Service.EntityService;
using MyLife.Service.MapperService;
using MyLife.Shared.DTOs;

namespace MyLife.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagController(TagServices tagServices,TagMapper tagMapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<TagDto?>> GetAll(Guid? guid = null, string? tag = null)
        {
            if (guid == null && tag == null)
            {
                var obj = await tagServices.TryReadListAsync();
                return obj.Select(e => tagMapper.ToDto(e));
            }
            else
            {
                var obj = await tagServices.TryReadAsync(guid, tag);
                return [obj!=null?tagMapper.ToDto(obj):null];
            }
        }
        [HttpPost]
        public async Task<Guid> AddTag(TagDto dto)
            =>await tagServices.TryCreateAsync(dto);

        [HttpPut]
        public async Task<Guid> UpdateTag(Guid guid, TagDto dto)
            =>await tagServices.TryUpdateAsync(guid, dto);

        [HttpDelete]
        public async Task<int> DeleteTag(Guid guid)
            =>await tagServices.TryDeleteAsync(guid);
    }
}
