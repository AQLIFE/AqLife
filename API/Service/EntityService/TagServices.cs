using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.MapperService;
using MyLife.Shared.Accident;
using MyLife.Shared.DTOs;

namespace MyLife.Service.EntityService
{
    public class TagServices(AppStorage storage, TagMapper tagMapper)
    {
        public async Task<IEnumerable<TagEntity>> TryReadListAsync()
             => storage.Tags.ToList();

        public async Task<TagEntity?> TryReadAsync(Guid? guid = null, string? tag = null)
        {
            if (guid != null)
                return await storage.Tags.FindAsync(guid);

            else if (tag != null)
                return await storage.Tags.Where(e => e.Name == tag).FirstOrDefaultAsync();
            return null;
        }


        public async Task<Guid> TryCreateAsync(TagDto dto)
        {
            if (dto == null) throw new OperateTransactionFailedException("数据无效");
            var entity = tagMapper.ToEntity(dto);
            if (storage.Tags.Any(e => e.Name == entity.Name)) throw new OperateTransactionFailedException("tag 已重复");
            await storage.Tags.AddAsync(entity);
            await storage.SaveChangesAsync();
            return entity.UID;
        }


        public async Task<Guid> TryUpdateAsync(Guid guid, TagDto dto)
        {
            if (dto == null || guid == Guid.Empty) throw new OperateTransactionFailedException("数据无效");
            if (storage.Tags.Any(e => e.UID == guid))
            {
                var entity = await TryReadAsync(guid);
                if (storage.Tags.Any(e => e.Name == dto.Name && e.UID != guid)) throw new OperateTransactionFailedException("tag 已重复");

                tagMapper.UpdateEntity(dto, entity!);
                //await storage.Tags.Update(entity);
                await storage.SaveChangesAsync();
                return guid;
            }
            throw new OperateTransactionFailedException("guid 不存在");

        }


        public async Task<int> TryDeleteAsync(Guid guid)
        {
            if (storage.Tags.Find(guid) is TagEntity entity) storage.Tags.Remove(entity);
            return await storage.SaveChangesAsync();
        }
    }
}
