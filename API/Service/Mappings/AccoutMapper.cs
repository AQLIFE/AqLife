using MyLife.Data.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class SubscriptionMapper : IGenericsMapper<SubscriptionEntity, SubscriptionDto>
    {
        [MapperIgnoreSource(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionDto Desensitization(SubscriptionEntity obj);

        [MapperIgnoreSource(nameof(SubscriptionDto.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionEntity Assembly(SubscriptionDto dto);

    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)] // 添加这一行

    public partial class AccountMapper(IGenericsMapper<SubscriptionEntity,SubscriptionDto> mapper) : IGenericsMapper<AccountEntity, AccountDto>
    {
        [MapperIgnoreSource(nameof(AccountEntity.UID))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        public partial AccountDto Desensitization(AccountEntity obj);


        [MapperIgnoreTarget(nameof(AccountEntity.UID))]
        [MapperIgnoreTarget(nameof(AccountEntity.IsValid))]
        public partial AccountEntity Assembly(AccountDto dto);


        public SubscriptionDto Convert(SubscriptionEntity entity)=>mapper.Desensitization(entity);

        public SubscriptionEntity Convert(SubscriptionDto dto)=>mapper.Assembly(dto);
    }
}
