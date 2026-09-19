namespace AqLife.Shared.Options
{
    public sealed class JwtOption
    {
        public required string SecretKey { init; get; }
        public required string Issuer { init; get; }
        public required string Audience { init; get; }
    }
}
