using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;

namespace WheresLou.Server.Kestrel.Transport.InlineSockets
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ILogger<Connection> _logger;

        public ConnectionFactory(ILogger<Connection> logger)
        {
            _logger = logger;
        }

        public TransportConnection Create(ConnectionContext connectionContext)
        {
            return new Connection(
                _logger,
                connectionContext);
        }
    }
}
