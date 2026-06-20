using MediatR;
using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Shared;
using MyLife.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Command
{
    //public record AccountData
    public record GetAccountQuery : IQuery<AccountDto>;
    public record LoginCommand(
    [Required] string AccountName,
    [Required] string SecretKey
) : ICommand<string>;

    public record DeleteAccountCommand(Guid UID) : IDeleteCommand, IMustCheckExistence<AccountEntity>;

    /// <summary>
    /// 创建账户
    /// </summary>
    /// <param name="Account"></param>
    /// <param name="Avatar"></param>
    /// <param name="SubAccountAvatar"></param>
    public record CreateAccountCommand(
       AccountFullDto Dto
    ) : ICreateCommand<Guid>, IRequireTransaction, IUploadRequest
    {
        public IEnumerable<IFormFile> GetFiles()
        {
            var files = new List<IFormFile>();
            if (Dto.Avatar != null) files.Add(Dto.Avatar);

            if (Dto.Subscriptions != null) files.AddRange(Dto.Subscriptions.Where(e => e.NewIconFile is not null).Select(x => x.NewIconFile!));
            return files;
        }
    }
    /// <summary>
    /// 更新账户基础信息
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Name"></param>
    /// <param name="Desc"></param>
    public record UpdateAccountProfileCommand(
    Guid UID,
    [Required, StringLength(20, MinimumLength = 3)]
    string Name,
    string? Desc,
    string SecretKey
    ) : IUpdateCommand<Guid>, IMustCheckExistence<AccountEntity>;

    /// <summary>
    /// 更新账户订阅列表
    /// </summary>
    /// <param name="AccountId"></param>
    /// <param name="Subscriptions"></param>
    public record UpdateAccountSubscriptionsCommand(
        Guid UID,
        IEnumerable<SubscriptionFullDto> Subscriptions
    ) : IUpdateCommand<Guid>,IUploadRequest, IMustCheckExistence<AccountEntity>
    {
        public IEnumerable<IFormFile> GetFiles() => Subscriptions.Where(x => x.NewIconFile != null).Select(e => e.NewIconFile).AsEnumerable()!;
    }

    /// <summary>
    /// 更新头像
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Avatar"></param>
    public record UpdateAccountAvatarCommand(
        Guid UID,
        IFormFile Avatar
    ) : IUpdateCommand<Guid>, IUploadRequest , IMustCheckExistence<AccountEntity>
    {
        public IEnumerable<IFormFile> GetFiles() => [Avatar];
    }
}
