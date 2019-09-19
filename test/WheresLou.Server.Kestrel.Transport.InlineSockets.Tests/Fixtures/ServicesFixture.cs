using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures
{
    public class ServicesFixture : IServiceProvider, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public ServicesFixture()
        {
            _serviceProvider = new ServiceCollection()
                .AddInlineSocketsTransport()
                .AddLogging()
                .BuildServiceProvider();
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
