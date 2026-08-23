using MyLife.Application.Abstractions.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Business.Account
{
    [Mapper]
    public partial class SubscriptionMapper : IViewMapper<SubscriptionEntity, SubscriptionDto>, ICreateMapper<SubscriptionEntity, ISubscription>
    {
        [MapperIgnoreSource(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.AID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionDto ToDto(SubscriptionEntity obj);

        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.AID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionEntity ToEntity(ISubscription source);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)] // 添加这一行
    public partial class AccountMapper(SubscriptionMapper subMapper)
        : IViewMapper<AccountEntity, AccountDto>, ICreateMapper<AccountEntity, CreateAccountCommand>
    {
        [MapperIgnoreSource(nameof(AccountEntity.UID))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        public partial AccountDto ToDto(AccountEntity source);

        [MapperIgnoreSource(nameof(CreateAccountCommand.ServerKey))]
        public  AccountEntity ToEntity(CreateAccountCommand command)=>new (command.Name,command.Desc,command.Pwd);

        private SubscriptionDto Convert(SubscriptionEntity entity) => subMapper.ToDto(entity);
        //private SubscriptionEntity Convert(SubscriptionDto dto) => subMapper.ToEntity(dto);

        private string Convert(Guid id) => id.ToString();
        private Guid Convert(string id) => Guid.TryParse(id, out var guid) ? guid : Guid.Empty; // string -> Guid
    }


}
