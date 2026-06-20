using MediatR;
using Microsoft.AspNetCore.Http;
using MyLife.Data.Repository;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.Account
{
    public class CreateAccountHandler(AppStorage storage, AccountMapper mapper,FileService fileService):IRequestHandler<CreateAccountCommand,Guid>
    {
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken ct)
        {
            var entity = mapper.ToEntity(request.Dto);
            var fileMetas = await fileService.TryCreateAsync(request.GetFiles(),ct);
            entity.Avatar = fileMetas.First();
            
            var sub = entity.Subscriptions.ToList();
            var list = fileMetas.Where(e => e != entity.Avatar).ToList();
            
            if(sub.Count == list.Count)
            for(int i = 0; i < sub.Count; i++)
            {
                    sub[i].SubscriptionIcon = list[i]; 
            }
            await storage.Account.AddAsync(entity, ct);            
            return entity.UID;
        }
    }
}
