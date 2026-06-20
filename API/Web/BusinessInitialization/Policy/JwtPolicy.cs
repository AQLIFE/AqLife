using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.Accident;
using MyLife.Shared.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MyLife.Web.BusinessInitialization.Policy
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
}
