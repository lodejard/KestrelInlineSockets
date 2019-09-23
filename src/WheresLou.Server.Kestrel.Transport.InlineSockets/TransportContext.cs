using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using System.Buffers;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;
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
