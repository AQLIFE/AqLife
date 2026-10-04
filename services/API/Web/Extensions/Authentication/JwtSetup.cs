using AqLife.Application.Abstractions.Authentication;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace AqLife.Web.Extensions.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public static class JwtSetup
    {
        /// <summary>
        /// 读取JWt 配置 并DI 到系统上
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationNotFoundException"></exception>
        public static IServiceCollection BindJwtPolicy(this IServiceCollection services,IConfiguration configuration)
        {
            //builder.Services.AddJwtOptions(configuration);

            var jwtSection = configuration.GetSection("Jwt");

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = JwtValidationParameters;
                option.Events = BearerEvents;
            });

            return services;
        }


        public static JwtBearerEvents BearerEvents { set; get; } = new JwtBearerEvents()
        {
            OnTokenValidated = async context =>
            {
                var concreteService = context.HttpContext.RequestServices.GetRequiredService<ITokenProvider<AccountEntity>>();
                if (!await concreteService.IsUserExistsAsync(context.Principal!, context.HttpContext.RequestAborted))
                {
                    context.Fail("该账户已被注销或不存在。");
                }

            }
        };
    }
}
