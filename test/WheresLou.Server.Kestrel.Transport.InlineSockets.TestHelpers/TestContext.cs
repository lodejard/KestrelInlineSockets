using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers.Fixtures;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers
{
    public class TestContext : IDisposable
    {
        private readonly ServiceProvider _fixtures;

        public TestContext()
        {
            _fixtures = new ServiceCollection()
                .AddSingleton<AppFixture>()
                .AddSingleton<EndPointFixture>()
                .AddSingleton<LoggingFixture>()
                .AddSingleton<ServerFixture>()
                .AddSingleton<ServicesFixture>()
                .AddSingleton<TimeoutFixture>()
                .AddSingleton<HttpClient>()
                .AddSingleton<OptionsFixture>()
                .BuildServiceProvider();
        }

        public AppFixture App => _fixtures.GetService<AppFixture>();
        public EndPointFixture EndPoint => _fixtures.GetService<EndPointFixture>();
        public LoggingFixture Logging => _fixtures.GetService<LoggingFixture>();
        public ServerFixture Server => _fixtures.GetService<ServerFixture>();
        public ServicesFixture Services => _fixtures.GetService<ServicesFixture>();
        public TimeoutFixture Timeout => _fixtures.GetService<TimeoutFixture>();
        public HttpClient Client => _fixtures.GetService<HttpClient>();
        public OptionsFixture Options => _fixtures.GetService<OptionsFixture>();

        public void Dispose()
        {
            _fixtures.Dispose();
        }
    }
}
