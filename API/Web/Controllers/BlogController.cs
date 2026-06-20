using Microsoft.AspNetCore.Mvc;
using MyLife.Data.Entities;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.DTOs;

namespace MyLife.Web.Controllers
{
    [ApiController,Route("[controller]")]
    public class BlogController(FileService fileService,FileMapper fileMapper):ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<FileMetadataDto>?> GetAll()
            =>await fileService.TryReadListAsync() is IEnumerable<FileMetaEntity> files && files.Any(e=>e!=null)?files.Select(e=>fileMapper.ToDto(e)):null;

        //public async Task<FileMetadataDto?> Get(GetFileMetadataQuery query,CancellationToken ct)
        //    => await fileService.TryReadAsync(query.UID, query.Title,ct) is FileMetaEntity entity?fileMapper.ToDto(entity):null;
    }
}
