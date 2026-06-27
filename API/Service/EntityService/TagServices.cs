using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;

namespace MyLife.Service.EntityService
{
    public class TagServices(AppStorage storage)
    {

        public async Task<IEnumerable<TagEntity?>> Search(CancellationToken ct, Guid? guid = null, string? tag = null)
        {
            if (guid != null)
                return [await storage.Tags.FindAsync(guid)];

            else if (tag != null)
                return await storage.Tags.Where(e => e.Name.Contains(tag)).ToListAsync();
            return await storage.Tags.ToListAsync();
        }


        public async Task<Guid> TryCreateAsync(CancellationToken ct, string Name, string? AliasName = null, bool IsCategory = false)
        {
            var entity = new TagEntity(Name, IsCategory, AliasName);
            await storage.Tags.AddAsync(entity);
            return entity.UID;
        }


        public async Task<Guid> TryUpdateAsync(CancellationToken ct, Guid guid, string Name, string? AliasName = null, bool IsCategory = false)
        {
            var entity = await Search(ct, guid);
            if (entity.FirstOrDefault() is TagEntity tag)
            {
                tag.AliasName = AliasName;
                tag.Name = Name;
                tag.IsCategory = IsCategory;
                return guid;
            }
            return Guid.Empty;
        }


        public async Task TryDeleteAsync(Guid guid, CancellationToken ct)
        {
            var entity = await Search(ct, guid);
            if (entity.FirstOrDefault() is TagEntity tag) storage.Tags.Remove(tag);
        }
    }
}
