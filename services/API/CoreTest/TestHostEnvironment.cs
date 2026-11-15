using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AqLife.CoreTest
{
    public sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";

        public string ApplicationName { get; set; } =
            typeof(TestHostEnvironment).Assembly.GetName().Name!;

        public string ContentRootPath { get; set; } =
            AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new PhysicalFileProvider(AppContext.BaseDirectory);
    }
}
