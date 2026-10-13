using AqLife.Application.Business.Account.Services;
using AqLife.Extensions.Application;
using AqLife.Extensions.Authentication;
using AqLife.Extensions.Configurations;
using AqLife.Extensions.Database;
using AqLife.Extensions.Infrastructure;
using AqLife.Extensions.Logging;
using AqLife.Infrastructure;
using AqLife.Shared.Options;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System.Security.Claims;
using System.Security.Principal;

namespace AqLife.CoreTest.IntegrationTest;

/// <summary>
/// 集成测试环境组装
/// </summary>
public class IntegrationTestFixture : IDisposable
{
    public ServiceProvider Services { get; }

    public IntegrationTestFixture()
    {

        var services = new ServiceCollection();
        var testEnvironment = new TestHostEnvironment();
        services.AddSingleton<IHostEnvironment>(testEnvironment);

        IConfigurationBuilder testConfiguration = new ConfigurationBuilder();
        var configuration = testConfiguration.AddAqLifeConfiguration(testEnvironment).Build();
        
        

        services.AddLogging();
        services.AddAqLifeSerilog(testEnvironment);
        services.AddFilePolicy(configuration, testEnvironment.ContentRootPath);
        
        services.AddJwtAuthentication(configuration);
        services.AddApplication();
        services.AddDatabase(configuration);
        services.AddInfrastructure(testEnvironment);
        ClaimsIdentity identity = new (
            new[]
            {
                new Claim(JwtClaimTypes.Id, Guid.Empty.ToString())
            },
            authenticationType: null);  // 非空才会 IsAuthenticated == true;此处仅用于测试,不需要有效信息
        HttpContextAccessor accessor = new ()
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
        services.AddSingleton<IHttpContextAccessor>(accessor);
        Services = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        Services.Dispose();
    }
    public async Task ResetDatabase()
    {
        using var scope = Services.CreateScope();

        var storage = scope.ServiceProvider
            .GetRequiredService<AppStorage>();

        storage.Accounts.RemoveRange(storage.Accounts);
        storage.Subscription.RemoveRange(storage.Subscription);
        storage.SystemStates.RemoveRange(storage.SystemStates);
        storage.File.RemoveRange(storage.File);

        await storage.SaveChangesAsync();
    }
}