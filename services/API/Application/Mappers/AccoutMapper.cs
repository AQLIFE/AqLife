using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.Account
{
    [Mapper]
    public partial class SubscriptionMapper : IViewMapper<SubscriptionEntity, SubscriptionDto>, ICreateMapper<SubscriptionEntity, ISubscription>
    {
        //[MapperIgnoreSource(nameof(SubscriptionEntity.Account))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreSource(nameof(SubscriptionEntity.AID))]
        public partial SubscriptionDto ToDto(SubscriptionEntity obj);

        //[MapperIgnoreTarget(nameof(SubscriptionEntity.Account))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.UID))]
        [MapperIgnoreTarget(nameof(SubscriptionEntity.AID))]
        public partial SubscriptionEntity ToEntity(ISubscription source);
    }

    [Mapper]
    public partial class AccountMapper(IViewMapper<SubscriptionEntity, SubscriptionDto> subMapper)
        : IViewMapper<AccountEntity, AccountDto>, ICreateMapper<AccountEntity, CreateAccountCommand>,
        ISearchMapper<AccountQuery, AccountDto, EntitySearchCriteria>
    {
        [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(AccountQuery query);// 特殊实现,该Query 仅用于查询当前用户,不需要 Keyword, 但仍然使用 EntitySearchCriteria 作为通用查询参数

        [MapperIgnoreSource(nameof(AccountEntity.UID))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        [MapperIgnoreSource(nameof(AccountEntity.LoginPasswordHash))]
        [MapperIgnoreSource(nameof(AccountEntity.LoginName))]
        [MapperIgnoreSource(nameof(AccountEntity.IsValid))]
        [MapProperty(nameof(AccountEntity.Subscriptions),nameof(AccountDto.Subscriptions))]
        [MapProperty(nameof(AccountEntity.Name), nameof(AccountDto.Name))]
        [MapProperty(nameof(AccountEntity.Desc), nameof(AccountDto.Desc))]
        [MapProperty(nameof(AccountEntity.Avatar), nameof(AccountDto.Avatar))]
        public partial AccountDto ToDto(AccountEntity source);

        // AccountEntity 的 Name/Desc 只有私有 setter，且密码需要由领域构造函数哈希。
        // 使用构造函数映射，避免 Mapperly 通过无参构造函数创建出空字段实体。
        public AccountEntity ToEntity(CreateAccountCommand command)
            => new(command.Name, command.Desc, command.Pwd);

        private IEnumerable<SubscriptionDto> Convert(ICollection<SubscriptionEntity> source) => source.Select(subMapper.ToDto);
    }
}
