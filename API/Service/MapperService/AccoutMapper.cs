using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class SubscriptionMapper : IGenMapper<SubscriptionEntity, SubscriptionDto>
    {
        [MapperIgnoreSource(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.SID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionDto ToDto(SubscriptionEntity obj);

        //[MapperIgnoreSource(nameof(SubscriptionDto.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        public partial SubscriptionEntity ToEntity(SubscriptionDto dto);

        //[MapperIgnoreSource(nameof(SubscriptionDto.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.SID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        public partial void UpdateEntity(SubscriptionDto dto, SubscriptionEntity entity);

    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)] // 添加这一行
    public partial class AccountMapper(SubscriptionMapper subMapper) : IGenMapper<AccountEntity, AccountDto>
    {
        [MapperIgnoreSource(nameof(AccountEntity.UID))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        public partial AccountDto ToDto(AccountEntity obj);




        [MapperIgnoreTarget(nameof(AccountEntity.UID))]
        [MapperIgnoreTarget(nameof(AccountEntity.IsValid))]
        public partial AccountEntity ToEntity(AccountDto dto);

        [MapperIgnoreTarget(nameof(AccountFullDto.Avatar))]
        public partial AccountEntity ToEntity(AccountFullDto dto);

        [MapperIgnoreTarget(nameof(AccountEntity.UID))]
        [MapperIgnoreTarget(nameof(AccountEntity.IsValid))]
        [MapperIgnoreTarget(nameof(AccountEntity.Subscriptions))] // ← 忽略集合，手动处理
        public partial void UpdateEntity(AccountDto dto, AccountEntity entity);
        private SubscriptionDto Convert(SubscriptionEntity entity) => subMapper.ToDto(entity);
        private SubscriptionEntity Convert(SubscriptionDto dto) => subMapper.ToEntity(dto);

        private string Convert(Guid id) => id.ToString();
        private Guid Convert(string id) => Guid.TryParse(id, out var guid) ? guid : Guid.Empty; // string -> Guid
    }


}
