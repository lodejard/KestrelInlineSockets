// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using WheresLou.Server.Kestrel.Transport.InlineSockets.TestHelpers;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Tests.Stubs;
using Xunit;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets.Tests
{
    public abstract class FactoryTests
    {
        [Fact]
        public async Task TransportFactoryCreate()
        {
            using var test = new TestContext();

            using var listener = test.Options.InlineSocketsOptions.CreateListener();

            Assert.NotNull(listener);

            await listener.DisposeAsync();
        }

        [Fact]
        public async Task ConnectionFactoryCreate()
        {
            using var test = new TestContext();

            var socket = new TestNetworkSocket();

            var connection = test.Options.InlineSocketsOptions.CreateConnection(socket);

            Assert.NotNull(connection);

            connection.Dispose();

            Assert.True(socket.IsDisposed);
        }
    }
}
