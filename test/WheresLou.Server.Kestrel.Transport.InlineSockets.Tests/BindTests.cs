// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;
using Xunit;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class BindTests
    {
        [Fact]
        public async Task BindCreatesSocketWhichAcceptsConnection()
        {
            using (var logging = new LoggingFixture())
            using (var services = new ServicesFixture(logging))
            using (var ep = new EndPointFixture())
            {
                var transportFactory = services.GetService<ITransportFactory>();

                ep.FindUnusedPort();

                var connectionDispatcher = new TestConnectionDispatcher();

                var transport = transportFactory.Create(
                    ep.EndPointInformation,
                    connectionDispatcher);

                await transport.BindAsync();

                using (var socket = new Socket(SocketType.Stream, ProtocolType.IP))
                {
                    Assert.Empty(connectionDispatcher.Connections);

                    socket.Connect(ep.EndPointInformation.IPEndPoint);
                    var localIPEndPoint = (IPEndPoint)socket.LocalEndPoint;

                    await Task.Delay(TimeSpan.FromMilliseconds(200));

                    var connection = Assert.Single(connectionDispatcher.Connections);

                    Assert.Equal(localIPEndPoint.Port, ((IHttpConnectionFeature)connection).RemotePort);
                }
            }
        }
    }
}
