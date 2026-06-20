using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyLife.Service.Command;

namespace MyLife.Service.Validators.BusinessValidator
{
    public class ValidityStrategy(AppStorage storage) :AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "重复的订阅记录";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command,CancellationToken ct)
        => command.Dto.Subscriptions.FirstOrDefault(e => storage.Subscription.Any(x => x.SubscriptionLink == e.SubscriptionLink)) is null;
    }

    public class ValidityIndexStrategy(AppStorage storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "无效的订阅图标";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command,CancellationToken ct)
        => command.Dto.Subscriptions.FirstOrDefault(e => storage.File.Any(x => x.UID == e.SubscriptionIcon)) is null;        
    }
}
