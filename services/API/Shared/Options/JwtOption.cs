namespace AqLife.Shared.Options
{
    public record JwtOption
    {
        public required string SecretKey { init; get; }
        public required string Issuer { init; get; }
        public required string Audience { init; get; }
    }
}
