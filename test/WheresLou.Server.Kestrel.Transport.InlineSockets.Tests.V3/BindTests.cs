// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers;
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
            using (var test = new TestContext()) 
            {
                test.EndPoint.FindUnusedPort();

                var transportFactory = test.Services.GetService<ITransportFactory>();

                var connectionDispatcher = new TestConnectionDispatcher();

                var transport = transportFactory.Create(
                    test.EndPoint.EndPointInformation,
                    connectionDispatcher);

                await transport.BindAsync();

                using (var socket = new Socket(SocketType.Stream, ProtocolType.IP))
                {
                    Assert.Empty(connectionDispatcher.Connections);

                    socket.Connect(test.EndPoint.EndPointInformation.IPEndPoint);
                    var localIPEndPoint = (IPEndPoint)socket.LocalEndPoint;

                    await Task.Delay(TimeSpan.FromMilliseconds(200));

                    var connection = Assert.Single(connectionDispatcher.Connections);

                    Assert.Equal(localIPEndPoint.Port, ((IHttpConnectionFeature)connection).RemotePort);
                }
            }
        }
    }
}
#endif
