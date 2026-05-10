namespace MyLife.Shared.Config
{
    public record JwtOption
    {
        public required string SecretKey { init; get; }
        public required string Issuer { init; get; }
        public required string Audience { init; get; }
    }


    /// <summary>
    /// 极简 版本的写法,缺少属性操作限制{set;get;},无法精细化操作
    /// </summary>
    /// <param name="SecretKey"></param>
    /// <param name="Issuer"></param>
    /// <param name="Audience"></param>
    //public record JwtOption(string SecretKey,string Issuer,string Audience);
}
