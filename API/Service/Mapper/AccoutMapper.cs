using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class SubscriptionMapper : IViewMapper<SubscriptionEntity, SubscriptionDto>,ICreateMapper<SubscriptionEntity,CreateSubscriptionCommand >
    {
        [MapperIgnoreSource(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.AID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionDto ToDto(SubscriptionEntity obj);

        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.AID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionEntity ToEntity(CreateSubscriptionCommand source);

        ////[MapperIgnoreSource(nameof(SubscriptionDto.SID))]
        //[MapperIgnoreTarget(nameof(SubscriptionEntity.AID))]
        //[MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        //[MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        //public partial void UpdateEntity(SubscriptionDto dto, SubscriptionEntity entity);

    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)] // 添加这一行
    public partial class AccountMapper(SubscriptionMapper subMapper)
        : IViewMapper<AccountEntity, AccountDto>,ICreateMapper<AccountEntity,CreateAccountCommand>
    {
        [MapperIgnoreSource(nameof(AccountEntity.UID))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        public partial AccountDto ToDto(AccountEntity source);

        public partial AccountEntity ToEntity(CreateAccountCommand command);

        private SubscriptionDto Convert(SubscriptionEntity entity) => subMapper.ToDto(entity);
        //private SubscriptionEntity Convert(SubscriptionDto dto) => subMapper.ToEntity(dto);

        private string Convert(Guid id) => id.ToString();
        private Guid Convert(string id) => Guid.TryParse(id, out var guid) ? guid : Guid.Empty; // string -> Guid
    }


}
