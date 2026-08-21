using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyLife.Shared.Options
{
    public record JwtOption
    {
        public required string SecretKey { init; get; }
        public required string Issuer { init; get; }
        public required string Audience { init; get; }
    }
}
