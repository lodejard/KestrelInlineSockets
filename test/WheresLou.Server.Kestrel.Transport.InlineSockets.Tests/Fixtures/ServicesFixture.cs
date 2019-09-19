using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                .AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>()
                .AddSingleton<IServer, KestrelServer>()
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
