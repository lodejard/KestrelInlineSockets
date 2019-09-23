using System.Buffers;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Factories;
using WheresLou.Server.Kestrel.Transport.InlineSockets.Internals;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class TransportFactory : ITransportFactory
    {
        private readonly TransportContext _context;

        public TransportFactory(
            ILogger<Transport> logger,
            IOptions<InlineSocketsTransportOptions> options,
            INetworkProvider networkProvider,
            IConnectionFactory connectionFactory)
        {
            _context = new TransportContext(
                logger,
                options.Value,
                networkProvider,
                connectionFactory);
        }

        public virtual ITransport Create(
            IEndPointInformation endPointInformation, 
            IConnectionDispatcher connectionDispatcher)
        {
            return new Transport(
                _context, 
                endPointInformation, 
                connectionDispatcher);
        }
    }
}
