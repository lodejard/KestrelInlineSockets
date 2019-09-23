// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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

        public ServicesFixture(LoggingFixture loggingFixture = null)
        {
            _serviceProvider = new ServiceCollection()
                .AddInlineSocketsTransport()
                .AddLogging(builder => loggingFixture?.ConfigureLogging(builder))
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
