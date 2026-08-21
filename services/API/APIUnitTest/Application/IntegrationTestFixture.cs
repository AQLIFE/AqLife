using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Application;
using MyLife.Infrastructure;
using MyLife.Infrastructure.Configuration;
using MyLife.Shared.Options;

namespace APIUnitTest.Application
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
    }
}
