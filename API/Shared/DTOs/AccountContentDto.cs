using Microsoft.AspNetCore.Http;

namespace MyLife.Shared.DTOs
{
    public record AccountContentDto(
            AccountDto Account,
            IFormFile Avatar,
            IFormFile[] SubAccountAvatar
        );
}
