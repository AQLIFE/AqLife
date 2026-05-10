using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLife.Data.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.Accident;
using MyLife.Shared.Config;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Duende.IdentityModel;


namespace MyLife.Web.Policy
{
    /// <summary>
    /// 
    /// </summary>
    public static class JwtPolicy
    {
        private static TokenValidationParameters JwtValidationParameters { get; set; }

        public static WebApplicationBuilder AddJwtPolicy(this WebApplicationBuilder builder)
        {
            var jwtSection = builder.Configuration.GetSection("Jwt");
            builder.Services.AddOptions<JwtOption>().Bind(jwtSection).ValidateOnStart();
            var jwtOption = jwtSection.Get<JwtOption>() ?? throw new OptionNotFoundException("无法从配置中加载 JwtOption，请检查 appsettings.json");

            JwtValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOption.Issuer,
                ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey))
            };

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option => option.TokenValidationParameters = JwtValidationParameters);



            return builder;
        }
    }

    /// <summary>
    /// 负责 Token 的生成和校验
    /// </summary>
    public class JwtProvider(IOptions<JwtOption> options) : IJwtProvider<AccountEntity>
    {
        public string CreateToken(AccountEntity account)
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Id, account.UID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var sourceToken = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims: claims, 
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(sourceToken);
        }

        public (bool IsValid, AccountEntity? account) ValidateToken(string token)
        {
            throw new Exception("JwtProvider.ValidateToken 方法尚未实现，请根据实际需求进行实现");
        }
    }

}
