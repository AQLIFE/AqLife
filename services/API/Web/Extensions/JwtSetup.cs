using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyLife.Application.Abstractions.Authentication;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;
using System.Text;


namespace MyLife.Web.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class JwtSetup
    {
        public static WebApplicationBuilder AddJwtPolicy(this WebApplicationBuilder builder)
        {
            var jwtSection = builder.Configuration.GetSection("Jwt");
            builder.Services.AddOptions<JwtOption>().Bind(jwtSection).ValidateOnStart();
            var jwtOption = jwtSection.Get<JwtOption>() ?? throw new ConfigurationNotFoundException("无法从配置中加载 JwtOption，请检查 appsettings.json");

            var JwtValidationParameters = new TokenValidationParameters
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
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = JwtValidationParameters;
                option.Events = BearerEvents;
            });

            return builder;
        }

        public static JwtBearerEvents BearerEvents { set; get; } = new JwtBearerEvents()
        {
            OnTokenValidated = async context =>
            {
                var concreteService = context.HttpContext.RequestServices.GetRequiredService<ITokenProvider<AccountEntity>>();
                if ( ! await concreteService.IsUserExistsAsync(context.Principal!,context.HttpContext.RequestAborted))
                {
                    context.Fail("该账户已被注销或不存在。");
                }

            }
        };
    }
}
