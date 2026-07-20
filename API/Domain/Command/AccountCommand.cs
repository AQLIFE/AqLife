using Microsoft.AspNetCore.Http;
using MyLife.Domain.CommandInterface;
using MyLife.Domain.Entities;
using MyLife.Shared;
using MyLife.Shared.IView;
using System.ComponentModel.DataAnnotations;


namespace MyLife.Domain.Command
{
    public record AccountQuery : IQuery<AccountDto?>;
    public record LoginCommand(
    [Required(ErrorMessage = "必填项:name")] string AccountName,
    [Required(ErrorMessage = "必填项:key")] string SecretKey
) : ICommand<string>;

    public record DeleteAccountCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<AccountEntity>;

    /// <summary>
    /// 创建账户
    /// </summary>
    /// <param name="Account"></param>
    /// <param name="Avatar"></param>
    /// <param name="SubAccountAvatar"></param>
    public record CreateAccountCommand(
        string Name,
        string? Desc,
        string pwd,
        string serverKey
    ) : ICreateCommand<string>, ISimpleAccountInfo
    { }

    /// <summary>
    /// 更新账户基础信息
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Name"></param>
    /// <param name="Desc"></param>
    public record UpdateAccountProfileCommand(
    Guid UID,
    [Required, StringLength(20, MinimumLength = 3,ErrorMessage ="账户名称长度必须介于3-20之间")]
    string Name,
    string? Desc
    ) : IUpdateCommand<string>, IRequireValidEntity<AccountEntity>;

    /// <summary>
    /// 更新账户订阅列表
    /// </summary>
    /// <param name="AccountId"></param>
    /// <param name="Subscriptions"></param>
    public record UpdateAccountSubscriptionsCommand(
        Guid UID,
        IEnumerable<CreateSubscriptionCommand> Subscriptions
    ) : IUpdateCommand<string>, IRequireValidEntity<AccountEntity>
    {
        //public IEnumerable<IFormFile> GetFiles() => Subscriptions.GetIEnumerableFiles();
    }

    /// <summary>
    /// 更新头像
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Avatar"></param>
    public record UpdateAccountAvatarCommand(
        Guid UID,
        [Required(ErrorMessage = "必须上传头像文件")] IFormFile Avatar
    ) : IUpdateCommand<string>, IHasFormFiles, IRequireValidEntity<AccountEntity>
    {
        public IEnumerable<IFormFile> GetFiles() => [Avatar];
    }
}
