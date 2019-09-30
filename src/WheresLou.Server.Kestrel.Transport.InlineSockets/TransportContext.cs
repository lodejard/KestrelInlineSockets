// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public struct TransportContext
    {
        public TransportContext(
            ITransportLogger logger,
            InlineSocketsTransportOptions options,
            INetworkProvider networkProvider,
            IConnectionFactory connectionFactory)
        {
            Logger = logger;
            Options = options;
            NetworkProvider = networkProvider;
            ConnectionFactory = connectionFactory;
        }

        public ITransportLogger Logger { get; }

        public InlineSocketsTransportOptions Options { get; }

        public INetworkProvider NetworkProvider { get; }

        public IConnectionFactory ConnectionFactory { get; }
    }
}
