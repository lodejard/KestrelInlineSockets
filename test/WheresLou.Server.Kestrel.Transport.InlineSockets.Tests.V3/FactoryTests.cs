// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;
using Xunit;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public abstract class FactoryTests
    {
        [Fact]
        public void TransportFactoryCreate()
        {
            using (var services = new ServicesFixture())
            {
                var transportFactory = services.GetService<ITransportFactory>();

                var transport = transportFactory.Create(new TestEndPointInformation(), new TestConnectionDispatcher());

                Assert.NotNull(transport);
            }
        }

        [Fact]
        public void ConnectionFactoryCreate()
        {
            using (var services = new ServicesFixture())
            {
                var connectionFactory = services.GetService<IInlineConnectionFactory>();

                var socket = new TestNetworkSocket();

                var connection = connectionFactory.CreateConnection(socket);

                Assert.NotNull(connection);

                connection.Dispose();

                Assert.True(socket.IsDisposed);
            }
        }
    }
}
#endif
