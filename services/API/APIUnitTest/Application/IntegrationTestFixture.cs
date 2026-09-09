using AqLife.Application;
using AqLife.Infrastructure;
using AqLife.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.APIUnitTest.Application
{
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
            services.AddLogging();
            services.AddJwtOptions(configuration);

            services.AddApplicationLayer();

            services.AddDataLayer(configuration);

            services.AddInfrastructure();
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
    }
}
