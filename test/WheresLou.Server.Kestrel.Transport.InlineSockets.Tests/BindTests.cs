using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;
using System.Net.Sockets;
using System.Net;
using System;
using Microsoft.AspNetCore.Http.Features;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class BindTests
    {
        [Fact]
        public async Task BindCreatesSocketWhichAcceptsConnection()
        {
            using (var services = new ServicesFixture())
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
