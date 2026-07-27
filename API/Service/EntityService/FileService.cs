//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using MyLife.Application;
//using MyLife.Domain.Entities;
//using MyLife.Service.Interfaces;
//using MyLife.Shared.Options;
//using MyLife.Shared.Tools;

//namespace MyLife.Service.EntityService
//{
//    public class FileService(
//        IOptions<FilePolicyOption> policy,
//        IApplicationDbContext storage,
//        IFileStorage fileStorage,
//        UploadContext upContext)//使用框架内置服务
//    {
        
//        public async Task<Guid> TryUpdateAsync(Guid guid, IFormFile file, CancellationToken ct)
//        {
//            FileMetaEntity? entity = await storage.File.FindAsync(guid,ct);//此时必定鉴权通过
//            if (entity is FileMetaEntity fileMeta)
//            {
//                fileMeta.FileHash = upContext.FileHashes.FirstOrDefault(e => e.Key == file).Value;
//                await fileStorage.SaveAsync(file.OpenReadStream(), file.FileName, ct);
//                return guid;
//            }
//            else
//                throw new FileNotFoundException("不存在的文件,无法更新");//重复逻辑,以防万一  
//        }

//        public async Task<Guid> TryUpdateAsync(Guid guid, IEnumerable<Guid> tags, CancellationToken ct)
//        {
//            FileMetaEntity? entity = await storage.File.FindAsync(guid, ct);//此时必定鉴权通过
//            if (entity is FileMetaEntity fileMeta )
//            {
//                var tagEntites = await storage.Tags.Where(e => tags.Contains(e.UID)).ToListAsync();
//                fileMeta.FileTags?.Clear();
//                fileMeta.FileTags ??= new List<FileTagEntity>();

//                // b. 建立新的契约映射
//                foreach (var tag in tagEntites)
//                {
//                    fileMeta.FileTags.Add(new FileTagEntity
//                    {
//                        FileId = guid,
//                        TagId = tag.UID,
//                        //Tag = tag,
//                        //File = fileMeta
//                    });
//                }

                
//                return guid;
//            }
//            else
//                throw new FileNotFoundException("不存在的文件,无法更新");//重复逻辑,以防万一  
//        }


//        public async Task<int> RemoveUnownedFile()
//        {
//            // 从 指定目录获取所有文件名称,并对数据库记录进行比对, 删除数据库中没有记录的文件, 避免垃圾文件占用存储空间
//            var groupFile = Directory.GetFiles(policy.Value.StoragePath);
//            var ownedFiles = await storage.File.Select(f => f.UID + f.Extension).ToListAsync();
//            var unownedFiles = groupFile.Where(f => !ownedFiles.Contains(Path.GetFileName(f)));

//            foreach (var file in unownedFiles)
//                File.Delete(file);

//            return unownedFiles.Count();
//        }
        
       
//    }
//}