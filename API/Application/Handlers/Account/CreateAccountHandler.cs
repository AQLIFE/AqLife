using MediatR;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using System.Xml.Linq;

namespace MyLife.Application.Handlers.Account
{
    public class CreateAccountHandler(AppStorage storage,AccountMapper mapper) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            AccountEntity entity = mapper.ToEntity(command);
            storage.Account.Add(entity);
            return entity.LoginName;
        }
    }
}
