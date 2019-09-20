using System;
using System.Threading;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Fixtures;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;
using Xunit;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public class FactoryTests
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
                var connectionFactory = services.GetService<IConnectionFactory>();

                var connection = connectionFactory.Create(new ConnectionContext(null, null, CancellationToken.None));

                Assert.NotNull(connection);
            }
        }
    }
}
