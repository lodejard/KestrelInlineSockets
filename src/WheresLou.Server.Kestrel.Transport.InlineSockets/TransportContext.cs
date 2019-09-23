// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct TransportContext
    {
        public TransportContext(
            ILogger<Transport> logger,
            InlineSocketsTransportOptions options,
            INetworkProvider networkProvider,
            IConnectionFactory connectionFactory)
        {
            Logger = logger;
            Options = options;
            NetworkProvider = networkProvider;
            ConnectionFactory = connectionFactory;
        }

        public ILogger<Transport> Logger { get; }

        public InlineSocketsTransportOptions Options { get; }

        public INetworkProvider NetworkProvider { get; }

        public IConnectionFactory ConnectionFactory { get; }
    }
}
