using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Shared.Utils;
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
        : IViewMapper<AccountEntity, AccountDto>, ICreateMapper<AccountEntity, CreateAccountCommand>
    {
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

        //[MapperIgnoreSource(nameof(CreateAccountCommand.Pwd))]
        //[MapperIgnoreTarget(nameof(AccountEntity.LoginPasswordHash))]
        [MapProperty(nameof(CreateAccountCommand.Pwd), nameof(AccountEntity.LoginPasswordHash)/*,Use =nameof(ConvertHashPwd)*/)]
        [MapperIgnoreSource(nameof(CreateAccountCommand.ServerKey))]
        [MapperIgnoreTarget(nameof(AccountEntity.Avatar))]
        [MapperIgnoreTarget(nameof(AccountEntity.IsValid))]
        [MapperIgnoreTarget(nameof(AccountEntity.UID))]
        [MapperIgnoreTarget(nameof(AccountEntity.LoginName))]
        [MapperIgnoreTarget(nameof(AccountEntity.Subscriptions))]
        public partial AccountEntity ToEntity(CreateAccountCommand command);

        private IEnumerable<SubscriptionDto> Convert(ICollection<SubscriptionEntity> source) => source.Select(subMapper.ToDto);

        //private string ConvertHashPwd(string source) => FastHash.GetSha256Hash(source);
    }


}
