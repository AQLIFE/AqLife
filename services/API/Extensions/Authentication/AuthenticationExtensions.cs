using AqLife.Application.Abstractions.Authentication;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AqLife.Extensions.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");

        var jwtOption = jwtSection.Get<JwtOption>()
            ?? throw new ConfigurationNotFoundException(
                "无法从配置中加载 JwtOption，请检查 appsettings.json");

        services
            .AddOptions<JwtOption>()
            .Bind(jwtSection)
            .ValidateOnStart();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOption.Issuer,
            ValidAudience = jwtOption.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOption.SecretKey))
        };

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenProvider =
                            context.HttpContext.RequestServices
                                .GetRequiredService<
                                    ITokenProvider<AccountEntity>>();

                        if (!await tokenProvider.IsUserExistsAsync(
                                context.Principal!,
                                context.HttpContext.RequestAborted))
                        {
                            context.Fail("该账户已被注销或不存在。");
                        }
                    }
                };
            });

        return services;
    }
}
