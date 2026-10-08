using AqLife.Application.Business.Account.Services;
using AqLife.Extensions.Application;
using AqLife.Extensions.Authentication;
using AqLife.Extensions.Database;
using AqLife.Extensions.Infrastructure;
using AqLife.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AqLife.CoreTest.Application;

/// <summary>
/// 集成测试环境组装
/// </summary>
public class IntegrationTestFixture : IDisposable
{
    public ServiceProvider Services { get; }

    public IntegrationTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        var environment = new TestHostEnvironment();

        services.AddSingleton<IHostEnvironment>(environment);
        services.AddLogging();

        services.AddJwtAuthentication(configuration);
        services.AddApplication();
        services.AddDatabase(configuration);
        services.AddInfrastructure(environment);
        

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

        await storage.SaveChangesAsync();
    }
}

public abstract class IntegrationTestBase(
    IntegrationTestFixture fixture)
{
    protected IServiceScope CreateScope()
    {
        return fixture.Services.CreateScope();
    }
    protected async Task<string?> EnsureSystemKeyAsync()
        =>await fixture.Services.EnsureAndGetSystemKeyAsync();
}
