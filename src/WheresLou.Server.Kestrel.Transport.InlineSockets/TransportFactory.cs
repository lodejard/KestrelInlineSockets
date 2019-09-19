using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class TransportFactory : ITransportFactory
    {
        private readonly ILogger<Transport> _logger;
        private readonly IConnectionFactory _connectionFactory;

        public TransportFactory(
            ILogger<Transport> logger,
            IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public virtual ITransport Create(
            IEndPointInformation endPointInformation, 
            IConnectionDispatcher dispatcher)
        {
            return new Transport(
                _logger, 
                _connectionFactory, 
                endPointInformation, 
                dispatcher);
        }
    }
}
